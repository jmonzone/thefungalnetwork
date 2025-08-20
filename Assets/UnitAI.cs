using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitAI : MonoBehaviour
{
    [Header("Wandering Settings")]
    public float wanderRadius = 10f;           // radius for random destinations
    public float minIdleTime = 1f;             // min pause
    public float maxIdleTime = 4f;             // max pause
    public float baseSpeed = 2f;               // base agent speed
    public float turnSpeedMin = 60f;           // min rotation speed
    public float turnSpeedMax = 120f;          // max rotation speed

    private NavMeshAgent agent;
    private Vector3 startPos;
    private float originalY;
    private Vector3 currentDestination;

    public event UnityAction<bool> OnIsMovingHasChanged;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true; // let NavMeshAgent handle rotation smoothly
        agent.speed = baseSpeed;
        startPos = transform.position;
        originalY = startPos.y;

        StartCoroutine(WanderRoutine());
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            // 1. Pick a random point on NavMesh
            currentDestination = GetReachableRandomDestination(transform.position, wanderRadius, NavMesh.AllAreas);
            agent.SetDestination(currentDestination);

            // 2. Slightly randomize speed and rotation for organic feel
            agent.speed = baseSpeed * Random.Range(0.8f, 1.2f);
            agent.angularSpeed = Random.Range(turnSpeedMin, turnSpeedMax);

            OnIsMovingHasChanged?.Invoke(true);

            // 3. Wait until reached destination or fail
            float timeout = 10f; // max time to reach destination
            float timer = 0f;

            while ((agent.pathPending || agent.remainingDistance > agent.stoppingDistance) && timer < timeout)
            {
                // If path is invalid or blocked
                if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    Debug.Log("Invalid");
                    break;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            OnIsMovingHasChanged?.Invoke(false);

            // 4. Idle for a random duration
            float idleTime = Random.Range(minIdleTime, maxIdleTime);
            float elapsed = 0f;
            while (elapsed < idleTime)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    private Vector3 GetReachableRandomDestination(Vector3 origin, float radius, int layermask, int maxAttempts = 10)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            // Pick a random point within the radius
            Vector3 randomPoint = origin + Random.insideUnitSphere * radius;

            // Project onto NavMesh
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, radius, layermask))
            {
                // Check if a valid path exists
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    Debug.Log("Reachable");
                    return hit.position; // reachable destination found
                }
                Debug.Log("Not Reachable");

            }
        }

        Debug.Log("Max Attempts");

        // Fallback: return current position if no valid point found
        return origin;
    }


    // Draw gizmo for destination
    private void OnDrawGizmos()
    {
        if (agent != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(currentDestination, 0.3f);
            Gizmos.DrawLine(transform.position, currentDestination);
        }
    }
}
