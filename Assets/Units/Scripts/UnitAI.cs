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

    private UnitDialogue dialogue;

    private UnitState currentState;
    private enum UnitState
    {
        WANDER,
        DIALOGUE,
        DESTINATION
    }

    public event UnityAction<bool> OnIsMovingHasChanged;

    private void Awake()
    {
        dialogue = GetComponent<UnitDialogue>();
        dialogue.OnDialogueStart += Dialogue_OnDialogueStart;
        dialogue.OnDialogueComplete += Dialogue_OnDialogueComplete;
    }

    private void Dialogue_OnDialogueComplete()
    {
        if (currentState == UnitState.DIALOGUE) StartWander();
    }

    private void Dialogue_OnDialogueStart()
    {
        currentState = UnitState.DIALOGUE; 
        StopAllCoroutines();
        agent.SetDestination(transform.position);
        transform.forward = Vector3.back;
        OnIsMovingHasChanged?.Invoke(false);
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true; // let NavMeshAgent handle rotation smoothly
        agent.speed = baseSpeed;

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(30, 70); // give variation so they don’t all “dance”

        startPos = transform.position;
        originalY = startPos.y;

        StartWander();
    }

    public void SetDestination(Vector3 destination, Vector3 direction)
    {
        currentState = UnitState.DESTINATION;

        StopAllCoroutines();

        agent.SetDestination(destination);
        transform.position = destination;
        transform.forward = direction;

        OnIsMovingHasChanged?.Invoke(false);
    }

    public void StartWander()
    {
        StopAllCoroutines();
        StartCoroutine(WanderRoutine());
    }

    private IEnumerator WanderRoutine()
    {
        currentState = UnitState.WANDER;

        // First step: from global zero
        yield return WanderStep(Vector3.zero, 1.5f);

        // Then loop from current position
        while (true)
        {
            yield return WanderStep(transform.position, wanderRadius);
        }
    }

    private IEnumerator WanderStep(Vector3 origin, float radius)
    {
        // 1. Pick a random point on NavMesh
        currentDestination = GetReachableRandomDestination(origin, radius, NavMesh.AllAreas);
        agent.SetDestination(currentDestination);

        // 2. Randomize speed/rotation for organic feel
        agent.speed = baseSpeed * Random.Range(0.8f, 1.2f);
        agent.angularSpeed = Random.Range(turnSpeedMin, turnSpeedMax);

        OnIsMovingHasChanged?.Invoke(true);

        // 3. Wait until reached destination or fail
        float timeout = 10f;
        float timer = 0f;

        while ((agent.pathPending || agent.remainingDistance > agent.stoppingDistance) && timer < timeout)
        {
            if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                Debug.Log("Invalid path");
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        OnIsMovingHasChanged?.Invoke(false);

        // 4. Idle for random duration
        float idleTime = Random.Range(minIdleTime, maxIdleTime);
        float elapsed = 0f;

        while (elapsed < idleTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    [SerializeField] private float gravitateStrength = 0.5f; // 0 = no gravitation, 1 = full pull to center

    private Vector3 GetReachableRandomDestination(Vector3 origin, float radius, int layermask, int maxAttempts = 10)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            // 1. Pick a random point in a sphere around origin
            Vector3 randomPoint = origin + Random.insideUnitSphere * radius;

            // 2. Apply gravitation toward the center
            randomPoint = Vector3.Lerp(randomPoint, Vector3.zero, gravitateStrength);

            // 3. Project onto NavMesh
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, radius, layermask))
            {
                // 4. Check if a valid path exists
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    return hit.position;
                }
            }
        }

        // Fallback
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
