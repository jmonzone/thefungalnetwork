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
    private List<Fish> allFish;
    private Fish targetFish;
    private FishPickup fishPickup;
    private NetworkFungal targetFungal;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        fungal = GetComponent<NetworkFungal>();

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
    }

    private void Update()
    {
        switch (currentState)
        {
            case FungalState.FIND_FISH:
                targetFish = allFish
                    .Where(fish => !fish.IsPickedUp.Value)
                    .OrderBy(fish => Vector3.Distance(transform.position, fish.transform.position)).FirstOrDefault();

                if (targetFish != null)
                {
                    agent.SetDestination(targetFish.transform.position);
                }

                if (fishPickup.Fish) SetState(FungalState.THROW_FISH);
                break;
            case FungalState.THROW_FISH:
                // Find the closest fungal that is NOT the player's own fungal
                targetFungal = pufferball.Players
                    .Where(player => player.Fungal != fungal) // Exclude self
                    .OrderBy(player => Vector3.Distance(transform.position, player.Fungal.transform.position))
                    .FirstOrDefault()?.Fungal;

                if (targetFungal != null)
                {
                    Vector3 playerPos = transform.position; // Player's current position
                    Vector3 fungalPos = targetFungal.transform.position; // Target fungal's position
                    float throwRange = targetFish.ThrowRange; // Max throw range

                    // Get the closest position to the player, but within throw range of the target fungal
                    Vector3 directionToPlayer = (playerPos - fungalPos).normalized;
                    Vector3 closestValidPosition = fungalPos + directionToPlayer * Mathf.Min(Vector3.Distance(fungalPos, playerPos), throwRange);

                    // Ensure it's on the NavMesh
                    if (NavMesh.SamplePosition(closestValidPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                    {
                        agent.SetDestination(hit.position);
                    }
                }
                break;


        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            switch (currentState)
            {
                case FungalState.FIND_FISH:
                    break;
                case FungalState.THROW_FISH:
                    fishPickup.Sling(targetFungal.transform.position);
                    SetState(FungalState.FIND_FISH);
                    break;
            }
        }
    }

    private void SetState(FungalState state)
    {
        currentState = state;

        switch (currentState)
        {
            case FungalState.FIND_FISH:
                break;
            case FungalState.THROW_FISH:
                break;
        }
    }

    private Vector3 GetRandomPointInRange()
    {
        Vector3 randomPos = origin + new Vector3(
            Random.Range(-range, range),
            0,
            Random.Range(-range, range)
        );

        // Ensure the point is on the NavMesh
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, range, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position; // Fallback if no valid position found
    }
}
