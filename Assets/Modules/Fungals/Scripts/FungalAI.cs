using System;
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

    [SerializeField] private GameReference pufferball;
    [SerializeField] private FungalState currentState;

    [SerializeField] private float minDashInterval = 2f; // Minimum time between dashes
    [SerializeField] private float maxDashInterval = 5f; // Maximum time between dashes

    private float lastDashTime = 0f;  // Stores the last dash time
    private float nextDashTime = 0f; // Next randomized dash time

    private NavMeshAgent agent;
    private NetworkFungal fungal;
    private FungalDash dash;
    private List<Fish> allFish = new List<Fish>();
    private Fish targetFish;
    private FishPickup fishPickup;
    private NetworkFungal targetFungal;
    private Coroutine fungalStateCoroutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        fungal = GetComponent<NetworkFungal>();
        dash = GetComponent<FungalDash>();
        allFish = FindObjectsOfType<Fish>().ToList();
        fishPickup = GetComponent<FishPickup>();
    }

    private void Start()
    {
        nextDashTime = dash.Cooldown.Cooldown;
        SetState(FungalState.FIND_FISH); // Start moving
    }

    private void OnEnable()
    {
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

        // Start the coroutine when the object is enabled
        fungalStateCoroutine = StartCoroutine(FungalStateMachine());

        pufferball.OnGameComplete += Pufferball_OnGameComplete;
    }

    private void OnDisable()
    {
        //Debug.Log($"FungalAI Disabling {name}");
        agent.enabled = false;
        // Stop the coroutine when the object is disabled
        if (fungalStateCoroutine != null)
        {
            StopCoroutine(fungalStateCoroutine);
            fungalStateCoroutine = null;
        }

        pufferball.OnGameComplete -= Pufferball_OnGameComplete;
    }

    private void Pufferball_OnGameComplete()
    {
        //Debug.Log($"Pufferball_OnGameComplete Disabling {name}");
        enabled = false;
    }

    private IEnumerator FungalStateMachine()
    {
        yield return new WaitForSeconds(4f);

        while (true)
        {
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
        while (currentState == FungalState.FIND_FISH)
        {
            targetFish = allFish
                .Where(fish => !fish.IsPickedUp.Value && IsOnNavMesh(fish.transform.position))
                .OrderBy(fish => Vector3.Distance(transform.position, fish.transform.position))
                .FirstOrDefault();

            if (targetFish != null)
            {
                yield return UseMoveAction(targetFish.transform.position);
            }

            if (fishPickup.Fish)
            {
                yield return new WaitForSeconds(1.5f);
                SetState(FungalState.THROW_FISH);
                yield break;
            }


            yield return null;
        }
    }

    private Vector3 GetDashTargetPosition(Vector3 targetPosition)
    {
        // Calculate the direction vector from the current position to the target
        Vector3 directionToTarget = targetPosition - transform.position;

        // Create a NavMeshPath and calculate the path to the target
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetPosition, path);

        // If the path is valid and complete
        if (path.status == NavMeshPathStatus.PathComplete && path.corners.Length > 1)
        {
            // Get the first corner after the current position (path.corners[1] is the next valid corner)
            Vector3 nextPathPoint = path.corners[1];

            // Calculate the distance of the full dash (distance between the current position and the next valid corner)
            float dashDistance = Vector3.Distance(transform.position, nextPathPoint);

            // If the dash distance is within the range, use the path
            if (dashDistance <= dash.Range)
            {
                return nextPathPoint;
            }
            else
            {
                // If the path is longer than the dash range, return the position based on the dash range
                return transform.position + directionToTarget.normalized * dash.Range;
            }
        }
        else
        {
            // If no valid path is found, fallback to a direct dash in the direction of the target
            return transform.position + directionToTarget.normalized * dash.Range;
        }
    }

    private IEnumerator UseMoveAction(Vector3 targetPosition)
    {
        agent.speed = fungal.Movement.CalculatedSpeed;
        agent.SetDestination(targetPosition);

        Vector3 dashTargetPosition = GetDashTargetPosition(targetPosition);

        float cooldownTime = dash.Cooldown.RemainingTime; // Get cooldown time

        // Check if the dash is ready (not on cooldown) and enough time has passed
        if (!dash.IsOnCooldown && Time.time >= lastDashTime + nextDashTime + cooldownTime)
        {
            StartCoroutine(dash.Cooldown.StartCooldown());

            agent.enabled = false;
            dash.CastAbility(dashTargetPosition);
            lastDashTime = Time.time;  // Update last dash time

            // Randomize the next dash interval within the defined range
            nextDashTime = UnityEngine.Random.Range(minDashInterval, maxDashInterval);

            yield return new WaitUntil(() => fungal.Movement.IsAtDestination);
            agent.enabled = true;

        }
    }

    private IEnumerator ThrowFish()
    {
        while (currentState == FungalState.THROW_FISH)
        {
            targetFungal = pufferball.Players
                .Where(player => player.Fungal != fungal && !player.Fungal.IsDead) // Exclude self
                .OrderBy(player => Vector3.Distance(transform.position, player.Fungal.transform.position))
                .FirstOrDefault()?.Fungal;

            if (targetFungal != null)
            {
                var playerPos = transform.position;
                var targetSlingPosition = targetFungal.transform.position + targetFungal.Movement.SpeedDelta * targetFungal.transform.forward;

                var directionToPlayer = (playerPos - targetSlingPosition).normalized;

                // Clamp the movement position within maxRange
                var targetMovePosition = targetSlingPosition;

                try
                {
                    targetMovePosition += directionToPlayer* Mathf.Min(Vector3.Distance(targetSlingPosition, playerPos), targetFish.ThrowFish.Range * 0.75f);
                }
                catch
                {
                    Debug.LogWarning($"Failed {targetFish} {targetFish.ThrowFish}");
                }

                // Check if the closest valid position is within range of the sling position
                if (NavMesh.SamplePosition(targetMovePosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                {
                    yield return UseMoveAction(hit.position);

                    if (Vector3.Distance(agent.transform.position, hit.position) < 0.05f)
                    {
                        // Once at the destination, sling the fish
                        fishPickup.Sling(targetSlingPosition);
                        SetState(FungalState.FIND_FISH);
                        yield break;
                    }
                }
            }

            yield return null;
        }
    }

    private void SetState(FungalState state)
    {
        currentState = state;
    }
}
