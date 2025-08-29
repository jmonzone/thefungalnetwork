using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Events;

public enum UnitState
{
    IDLE,
    WANDER,
    JOB,
    DIALOGUE,
}

public enum UnitJob
{
    FORAGE,
    GARDEN
}

public interface IJob
{
    public bool IsAble { get; }
    public bool IsMoving { get; }
    public Vector3 TargetPosition { get; }
    public event UnityAction OnIsAbleChanged;
    public event UnityAction OnIsMovingChanged;
}

[RequireComponent(typeof(NavMeshAgent))]
public class UnitAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PartyReference partyReference;

    [Header("Wandering Settings")]
    [SerializeField] private float wanderRadius = 10f;           // radius for random destinations
    [SerializeField] private float minIdleTime = 1f;             // min pause
    [SerializeField] private float maxIdleTime = 4f;             // max pause
    [SerializeField] private float baseSpeed = 2f;               // base agent speed
    [SerializeField] private float turnSpeedMin = 60f;           // min rotation speed
    [SerializeField] private float turnSpeedMax = 120f;          // max rotation speed
    [SerializeField] private float gravitateStrength = 0.5f; // 0 = no gravitation, 1 = full pull to center

    private NavMeshAgent agent;
    private Vector3 currentDestination;

    private UnitDialogue dialogue;

    [SerializeField] private UnitState currentState;
    [SerializeField] private float idleTargetTime;
    [SerializeField] private float idleElapsedTime;

    [SerializeField] private UnitJob currentJob;

    private IJob jobScript;

    public event UnityAction<bool> OnIsMovingHasChanged;

    private void Awake()
    {
        dialogue = GetComponent<UnitDialogue>();
        dialogue.OnDialogueStart += Dialogue_OnDialogueStart;
        dialogue.OnDialogueComplete += Dialogue_OnDialogueComplete;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true; // let NavMeshAgent handle rotation smoothly
        agent.speed = baseSpeed;

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(30, 70); // give variation so they don’t all “dance”

        jobScript = currentJob switch
        {
            UnitJob.FORAGE => GetComponent<UnitForage>(),
            UnitJob.GARDEN => GetComponent<UnitGarden>(),
            _ => GetComponent<UnitForage>(),
        };

        jobScript.OnIsAbleChanged += SetDefaultState;
        jobScript.OnIsMovingChanged += UpdateIsMoving;
    }

    private void Start()
    {
        SetDefaultState();
        StartCoroutine(StateRoutine());
    }

    private void SetDefaultState()
    {
        if (currentState == UnitState.DIALOGUE) return;
        if (jobScript.IsAble) SetCurrentState(UnitState.JOB);
        else SetCurrentState(UnitState.IDLE);
    }

    private void SetCurrentState(UnitState state)
    {
        currentState = state;

        UpdateIsMoving();

        agent.speed = baseSpeed * Random.Range(0.8f, 1.2f);
        agent.angularSpeed = Random.Range(turnSpeedMin, turnSpeedMax);

        switch (state)
        {

            case UnitState.IDLE:
                idleTargetTime = Random.Range(minIdleTime, maxIdleTime);
                idleElapsedTime = 0;
                break;
            case UnitState.DIALOGUE:
                transform.forward = Vector3.back;
                break;
            case UnitState.WANDER:
                currentDestination = GetReachableRandomDestination(transform.position, wanderRadius, NavMesh.AllAreas);
                agent.SetDestination(currentDestination);
                break;
        }
    }

    private IEnumerator StateRoutine()
    {
        while (true)
        {
            switch (currentState)
            {
                case UnitState.JOB:
                    agent.SetDestination(jobScript.TargetPosition);
                    break;
                case UnitState.WANDER:
                    Vector3 lookDir = (currentDestination - transform.position).normalized;
                    lookDir.y = 0;
                    if (lookDir.sqrMagnitude > 0.001f)
                    {
                        Quaternion lookRotation = Quaternion.LookRotation(lookDir);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
                    }

                    if (Vector3.Distance(currentDestination, transform.position) < 0.5f)
                    {
                        SetCurrentState(UnitState.IDLE);
                    }
                    break;

                case UnitState.IDLE:
                    idleElapsedTime += Time.deltaTime;

                    if (idleElapsedTime > idleTargetTime)
                    {
                        SetCurrentState(UnitState.WANDER);
                    }
                    break;
            }

            yield return null;
        }
    }

    private void UpdateIsMoving()
    {
        agent.isStopped = currentState switch
        {
            UnitState.IDLE => true,
            UnitState.DIALOGUE => true,
            UnitState.JOB => !jobScript.IsMoving,
            _ => false,
        };

        OnIsMovingHasChanged?.Invoke(!agent.isStopped);
    }

    private void Dialogue_OnDialogueComplete()
    {
        SetDefaultState();
    }

    private void Dialogue_OnDialogueStart()
    {
        SetCurrentState(UnitState.DIALOGUE);
    }

    public void SetDestination(Vector3 destination, Vector3 direction)
    {
        //currentState = UnitState.DESTINATION;

        //StopAllCoroutines();

        //agent.SetDestination(destination);
        //transform.position = destination;
        //transform.forward = direction;

        //OnIsMovingHasChanged?.Invoke(false);
    }


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
            Gizmos.DrawSphere(agent.destination, 0.3f);
            Gizmos.DrawLine(transform.position, currentDestination);
        }
    }
}
