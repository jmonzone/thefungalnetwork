using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FungalAI : MonoBehaviour
{
    private enum FungalState
    {
        FIND_FISH,
        THROW_FISH,
    }

    [SerializeField] private GameReference game;
    [SerializeField] private FungalState currentState;

    [SerializeField] private float minDashInterval = 2f; // Minimum time between dashes
    [SerializeField] private float maxDashInterval = 5f; // Maximum time between dashes

    private float lastDashTime = 0f;  // Stores the last dash time
    private float nextDashTime = 0f; // Next randomized dash time

    private NavMeshAgent agent;
    private FungalController fungal;
    private Ability ability;
    private List<FishController> allFish = new List<FishController>();
    private FishController targetFish;
    private FishPickup fishPickup;
    private List<FungalController> allFungals = new List<FungalController>();
    private FungalController targetFungal;
    private Coroutine fungalStateCoroutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        fungal = GetComponent<FungalController>();

        allFish = FindObjectsOfType<FishController>().ToList();
        allFungals = FindObjectsOfType<FungalController>().ToList();

        fishPickup = GetComponent<FishPickup>();

        var fungalController = GetComponent<FungalController>();
        fungalController.OnDeath += _ => StopAI();
        fungalController.OnRespawnComplete += () => StartAI();
    }

    private void Start()
    {
        var abilityTemplate = fungal.Data.Ability;
        ability = Instantiate(abilityTemplate);
        ability.Initialize(fungal);

        //nextDashTime = dash.Cooldown.Cooldown;
        //StartAI();
    }

    public void StartAI()
    {
        //if isOwwner && is ai
        agent.enabled = true;

        // Ensure AI starts on the NavMesh
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position); // Teleport AI to valid NavMesh point
        }
        else
        {
            Debug.LogError("AI is outside the NavMesh!");
        }

        agent.isStopped = false; // Make sure it's not paused

        SetState(FungalState.FIND_FISH); // Start moving

        // Start the coroutine when the object is enabled
        fungalStateCoroutine = StartCoroutine(FungalStateMachine());
    }

    public void StopAI()
    {
        agent.enabled = false;

        // Stop the coroutine when the object is disabled
        if (fungalStateCoroutine != null)
        {
            StopCoroutine(fungalStateCoroutine);
            fungalStateCoroutine = null;
        }
    }


    private IEnumerator FungalStateMachine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            if (!(ability is DirectionalAbility) && !ability.IsOnCooldown)
            {
                ability.CastAbility();
            }

            switch (currentState)
            {
                case FungalState.FIND_FISH:
                    yield return FindAndPickUpFish();
                    break;
                case FungalState.THROW_FISH:
                    yield return ThrowFish();
                    break;
            }

            yield return null;
        }
    }

    // Helper function to check if the position is valid on the NavMesh
    private bool IsOnNavMesh(Vector3 position)
    {
        return NavMesh.SamplePosition(position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas);
    }

    private IEnumerator FindAndPickUpFish()
    {
        targetFish = allFish
                 .Where(fish => !fish.IsPickedUp && IsOnNavMesh(fish.transform.position))
                 .OrderBy(fish => Vector3.Distance(transform.position, fish.transform.position))
                 .FirstOrDefault();

        if (targetFish != null)
        {
            yield return UseMoveAction(targetFish.transform.position);
        }

        if (fishPickup.Fish)
        {
            SetState(FungalState.THROW_FISH);
        }

        yield return null;
    }

    private Vector3 GetDashTargetPosition(IMovementAbility movementAbility, Vector3 targetPosition)
    {
        float maxRange = movementAbility.Range;
        Vector3 origin = transform.position;

        // Direction from origin to target
        Vector3 direction = (targetPosition - origin).normalized;
        Vector3 intendedPosition = origin + direction * maxRange;

        // Sample the NavMesh to find the nearest valid point within a radius
        if (NavMesh.SamplePosition(intendedPosition, out NavMeshHit hit, maxRange, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // If no valid position found, return current position
        return origin;
    }


    private IEnumerator UseMoveAction(Vector3 targetPosition)
    {
        agent.speed = fungal.Movement.CalculatedSpeed;
        agent.SetDestination(targetPosition);

        if (ability is IMovementAbility movementAbility && ability is DirectionalAbility directionalAbility)
        {

            float cooldownTime = ability.Cooldown.RemainingTime; // Get cooldown time

            // Check if the dash is ready (not on cooldown) and enough time has passed
            if (!ability.IsOnCooldown && Time.time >= lastDashTime + nextDashTime + cooldownTime)
            {
                Debug.Log("Fungal AI Casting ability");

                agent.enabled = false;

                Vector3 dashTargetPosition = GetDashTargetPosition(movementAbility, targetPosition);
                directionalAbility.CastAbility(dashTargetPosition);

                lastDashTime = Time.time;  // Update last dash time

                // Randomize the next dash interval within the defined range
                nextDashTime = Random.Range(minDashInterval, maxDashInterval);

                yield return new WaitUntil(() => fungal.Movement.IsAtDestination);
                agent.enabled = true;
            }
        }
        
    }

    private IEnumerator ThrowFish()
    {
        targetFungal = allFungals
                .Where(fungal => this.fungal != fungal && !fungal.IsDead) // Exclude self
                .OrderBy(fungal => Vector3.Distance(transform.position, fungal.transform.position))
                .FirstOrDefault();

        if (targetFungal != null && fishPickup.Fish)
        {
            var playerPos = transform.position;
            var targetSlingPosition = targetFungal.transform.position + targetFungal.Movement.SpeedDelta * targetFungal.transform.forward;

            var directionToPlayer = (playerPos - targetSlingPosition).normalized;

            // Clamp the movement position within maxRange
            var targetMovePosition = targetSlingPosition;

            targetMovePosition += directionToPlayer * Mathf.Min(Vector3.Distance(targetSlingPosition, playerPos), fishPickup.Fish.ThrowFish.Range * 0.75f);

            // Check if the closest valid position is within range of the sling position
            if (NavMesh.SamplePosition(targetMovePosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                yield return UseMoveAction(hit.position);

                if (Vector3.Distance(agent.transform.position, hit.position) < 0.05f)
                {
                    // Once at the destination, sling the fish
                    fishPickup.Sling(targetSlingPosition);
                }
            }
        }
        else
        {
            SetState(FungalState.FIND_FISH);
        }
    }

    private void SetState(FungalState state)
    {
        currentState = state;
    }
}
