using UnityEngine;
using UnityEngine.AI;

public class FungalAI : MonoBehaviour
{
    public float range = 5f; // Radius from origin
    private Vector3 origin;  // Initial spawn position
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        origin = Vector3.zero; // Use the AI's initial spawn position

        // Ensure AI starts on the NavMesh
        if (NavMesh.SamplePosition(origin, out NavMeshHit hit, range, NavMesh.AllAreas))
        {
            agent.Warp(hit.position); // Teleport AI to valid NavMesh point
        }
        else
        {
            Debug.LogError("AI is outside the NavMesh!");
            return;
        }

        agent.isStopped = false; // Make sure it's not paused
        SetNewDestination(); // Start moving
    }


    private void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SetNewDestination(); // Pick a new random destination
        }
    }

    private void SetNewDestination()
    {
        Vector3 randomPoint = GetRandomPointInRange();
        Debug.Log($"Setting destination {randomPoint}");
        agent.SetDestination(randomPoint);
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
