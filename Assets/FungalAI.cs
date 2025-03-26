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
    private List<Fish> allFish;
    private Fish targetFish;
    private FishPickup fishPickup;
    private NetworkFungal targetFungal;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        fungal = GetComponent<NetworkFungal>();
        dash = GetComponent<FungalDash>();

        origin = Vector3.zero; // Use the AI's initial spawn position

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
        SetState(FungalState.FIND_FISH); // Start moving

        allFish = FindObjectsOfType<Fish>().ToList();
        fishPickup = GetComponent<FishPickup>();

        Debug.Log("Starting");
    }

    private Coroutine fungalStateCoroutine;

    private void OnEnable()
    {
        // Start the coroutine when the object is enabled
        fungalStateCoroutine = StartCoroutine(FungalStateMachine());
    }

    private void OnDisable()
    {
        // Stop the coroutine when the object is disabled
        if (fungalStateCoroutine != null)
        {
            StopCoroutine(fungalStateCoroutine);
            fungalStateCoroutine = null;
        }
    }

    private IEnumerator FungalStateMachine()
    {
        while (true)
        {
            switch (currentState)
            {
                case FungalState.FIND_FISH:
                    yield return StartCoroutine(FindAndPickUpFish());
                    break;
                case FungalState.THROW_FISH:
                    yield return StartCoroutine(ThrowFish());
                    break;
            }
            yield return null;
        }
    }

    private IEnumerator FindAndPickUpFish()
    {
        while (currentState == FungalState.FIND_FISH)
        {
            targetFish = allFish
                .Where(fish => !fish.IsPickedUp.Value)
                .OrderBy(fish => Vector3.Distance(transform.position, fish.transform.position))
                .FirstOrDefault();

            if (targetFish != null)
                agent.SetDestination(targetFish.transform.position);

            Vector3 dashTargetPosition = GetDashTargetPosition(targetFish.transform.position);

            // Only cast the ability if it's not on cooldown
            CastDashAbility(dashTargetPosition);

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

    private void CastDashAbility(Vector3 targetPosition)
    {
        if (!dash.IsOnCooldown)
        {
            dash.CastAbility(targetPosition);
            StartCoroutine(dash.Cooldown.StartCooldown());
        }
    }
    private IEnumerator ThrowFish()
    {
        targetFungal = pufferball.Players
            .Where(player => player.Fungal != fungal) // Exclude self
            .OrderBy(player => Vector3.Distance(transform.position, player.Fungal.transform.position))
            .FirstOrDefault()?.Fungal;

        if (targetFungal != null)
        {
            Vector3 playerPos = transform.position;
            Vector3 fungalPos = targetFungal.transform.position;
            float throwRange = targetFish.ThrowRange;

            Vector3 directionToPlayer = (playerPos - fungalPos).normalized;
            Vector3 closestValidPosition = fungalPos + directionToPlayer * Mathf.Min(Vector3.Distance(fungalPos, playerPos), throwRange);

            if (NavMesh.SamplePosition(closestValidPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
                    yield return null;
            }
        }

        fishPickup.Sling(targetFungal.transform.position + targetFungal.transform.forward * 1.5f);
        SetState(FungalState.FIND_FISH);
    }

    private void Move()
    {

    }


    private void SetState(FungalState state)
    {
        currentState = state;
    }
}
