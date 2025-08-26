using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Events;
using System.Linq;

public enum UnitState
{
    IDLE,
    WANDER,
    COLLECT_SPORE,
    HARVEST_PLANT,
}

[RequireComponent(typeof(NavMeshAgent))]
public class UnitAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private SporeReference sporeReference;
    [SerializeField] private BuildSystem buildReference;

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
    

    public event UnityAction<bool> OnIsMovingHasChanged;

    private void Awake()
    {
        dialogue = GetComponent<UnitDialogue>();
        //dialogue.OnDialogueStart += Dialogue_OnDialogueStart;
        //dialogue.OnDialogueComplete += Dialogue_OnDialogueComplete;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true; // let NavMeshAgent handle rotation smoothly
        agent.speed = baseSpeed;

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(30, 70); // give variation so they don’t all “dance”

        SetState(UnitState.WANDER);
        StartCoroutine(StateRoutine());
    }

    private void OnEnable()
    {
        sporeReference.OnSporeControllersChanged += OnSporeControllersChanged;
    }

    private void OnDisable()
    {
        sporeReference.OnSporeControllersChanged -= OnSporeControllersChanged;
    }

    private SporeController targetSpore;

    private void OnSporeControllersChanged()
    {
        if (TryGetClosestSpore(out SporeController closestSpore))
        {
            targetSpore = closestSpore;
            SetState(UnitState.COLLECT_SPORE);
        }
        else
        {
            targetSpore = closestSpore;
            SetState(UnitState.WANDER);
        }
    }

    private bool TryGetClosestSpore(out SporeController closestSpore)
    {
        if (sporeReference.SporeControllers.Count > 0)
        {
            closestSpore = sporeReference.SporeControllers[0];
            foreach(var spore in sporeReference.SporeControllers)
            {
                if (Vector3.Distance(transform.position, spore.transform.position) < Vector3.Distance(transform.position, closestSpore.transform.position))
                {
                    closestSpore = spore;
                }
            }
            SetState(UnitState.COLLECT_SPORE);
            return true;
        }
        else
        {
            closestSpore = null;
            return false;
        }
    }

    private void SetState(UnitState state)
    {
        currentState = state;

        agent.isStopped = state switch
        {
            UnitState.IDLE => true,
            _ => false,
        };

        OnIsMovingHasChanged?.Invoke(!agent.isStopped);

        agent.speed = baseSpeed * Random.Range(0.8f, 1.2f);
        agent.angularSpeed = Random.Range(turnSpeedMin, turnSpeedMax);

        switch (state)
        {
            case UnitState.COLLECT_SPORE:
                break;
            case UnitState.WANDER:
                currentDestination = GetReachableRandomDestination(transform.position, wanderRadius, NavMesh.AllAreas);
                agent.SetDestination(currentDestination);
                break;
            case UnitState.IDLE:
                idleTargetTime = Random.Range(minIdleTime, maxIdleTime);
                idleElapsedTime = 0;
                break;
        }
    }

    private IEnumerator StateRoutine()
    {
        while (true)
        {
            switch (currentState)
            {
                case UnitState.COLLECT_SPORE:
                    agent.SetDestination(targetSpore.transform.position);
                    if (Vector3.Distance(targetSpore.transform.position, transform.position) < 0.5f)
                    {
                        targetSpore.Collect();

                        if (TryGetClosestSpore(out SporeController closestSpore))
                        {
                            targetSpore = closestSpore;
                            SetState(UnitState.COLLECT_SPORE);
                        }
                        else
                        {
                            targetSpore = closestSpore;
                            SetState(UnitState.IDLE);
                        }
                    }
                    break;
                case UnitState.WANDER:
                    if (Vector3.Distance(currentDestination, transform.position) < 0.5f)
                    {
                        SetState(UnitState.IDLE);
                    }
                    break;

                case UnitState.IDLE:
                    idleElapsedTime += Time.deltaTime;

                    if (idleElapsedTime > idleTargetTime)
                    {
                        SetState(UnitState.WANDER);
                    }
                    break;
            }

            yield return null;
        }
    }

    private void Dialogue_OnDialogueComplete()
    {
        //if (currentState == UnitState.DIALOGUE) StartWander();
    }

    private void Dialogue_OnDialogueStart()
    {
        //currentState = UnitState.DIALOGUE; 
        //StopAllCoroutines();
        //agent.SetDestination(transform.position);
        //transform.forward = Vector3.back;
        //OnIsMovingHasChanged?.Invoke(false);
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
            Gizmos.DrawSphere(currentDestination, 0.3f);
            Gizmos.DrawLine(transform.position, currentDestination);
        }
    }
}
