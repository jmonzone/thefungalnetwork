using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FungalAI : MonoBehaviour
{
    public float range = 5f; // Radius from origin
    private Vector3 origin;  // Initial spawn position
    private NavMeshAgent agent;
    [SerializeField] private PufferballReference pufferball;
    private enum FungalState
    {
        FIND_FISH,
        THROW_FISH,
    }

    [SerializeField] private FungalState currentState;

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
        origin = Vector3.zero; // Use the AI's initial spawn position
    }

    private void Start()
    {
        SetState(FungalState.FIND_FISH); // Start moving
    }

    private void OnEnable()
    {
        agent.enabled = true;

        // Ensure AI starts on the NavMesh
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, range, NavMesh.AllAreas))
        {
            agent.Warp(hit.position); // Teleport AI to valid NavMesh point
        }
        else
        {
            Debug.LogError("AI is outside the NavMesh!");
            return;
        }

        agent.isStopped = false; // Make sure it's not paused

        // Start the coroutine when the object is enabled
        fungalStateCoroutine = StartCoroutine(FungalStateMachine());
    }

    private void OnDisable()
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
        // Calculate the direction vector from the current transform to the target fish
        Vector3 directionToTarget = targetPosition - transform.position;

        // Use NavMeshAgent to calculate the best path to the target
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetPosition, path);

        // If the path is valid and complete
        if (path.status == NavMeshPathStatus.PathComplete && path.corners.Length > 1)
        {
            // Get the next corner on the path, which is the next point along the valid path
            Vector3 nextPathPoint = path.corners[1];

            // Check if the next corner is within 5 units
            Vector3 directionToPathPoint = nextPathPoint - transform.position;
            return directionToPathPoint.magnitude <= dash.Range
                ? nextPathPoint
                : transform.position + directionToPathPoint.normalized * 5f;
        }
        else
        {
            // If no valid path is found, fallback to the direct direction towards the fish
            return transform.position + directionToTarget.normalized * 5f;
        }
    }

    private float lastDashTime = 0f;  // Stores the last dash time
    [SerializeField] private float minDashInterval = 2f; // Minimum time between dashes
    [SerializeField] private float maxDashInterval = 5f; // Maximum time between dashes
    private float nextDashTime = 0f; // Next randomized dash time

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
            nextDashTime = Random.Range(minDashInterval, maxDashInterval);

            yield return new WaitUntil(() => fungal.Movement.IsAtDestination);
            agent.enabled = true;

        }
    }

    private IEnumerator ThrowFish()
    {
        while (currentState == FungalState.THROW_FISH)
        {
            targetFungal = pufferball.Players
                .Where(player => player.Fungal != fungal) // Exclude self
                .OrderBy(player => Vector3.Distance(transform.position, player.Fungal.transform.position))
                .FirstOrDefault()?.Fungal;

            if (targetFungal != null)
            {
                var playerPos = transform.position;
                var targetSlingPosition = targetFungal.transform.position + targetFungal.Movement.SpeedDelta * targetFungal.transform.forward;


                var directionToPlayer = (playerPos - targetSlingPosition).normalized;

                // Clamp the movement position within maxRange
                var targetMovePosition = targetSlingPosition + directionToPlayer * Mathf.Min(Vector3.Distance(targetSlingPosition, playerPos), targetFish.ThrowRange * 0.75f);

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
