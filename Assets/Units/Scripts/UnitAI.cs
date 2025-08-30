using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Events;

public enum UnitState
{
    JOB,
    DIALOGUE,
    ACTIVITY,
    FOLLOW
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
    [SerializeField] private PlayerReference playerReference;

    private NavMeshAgent agent;
    private Vector3 currentDestination;

    private UnitDialogue dialogue;
    private UnitFollow unitFollow;

    [SerializeField] private UnitState currentState;

    //private void Awake()
    //{
    //    dialogue = GetComponent<UnitDialogue>();
    //    dialogue.OnDialogueStart += Dialogue_OnDialogueStart;
    //    dialogue.OnDialogueComplete += Dialogue_OnDialogueComplete;

    //    unitFollow = GetComponent<UnitFollow>();

    //    agent = GetComponent<NavMeshAgent>();
    //    agent.updateRotation = true; // let NavMeshAgent handle rotation smoothly
    //    agent.speed = baseSpeed;

    //    agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
    //    agent.avoidancePriority = Random.Range(30, 70); // give variation so they don’t all “dance”

    //    jobScript = currentJob switch
    //    {
    //        UnitJob.FORAGE => GetComponent<UnitForage>(),
    //        UnitJob.GARDEN => GetComponent<UnitGarden>(),
    //        _ => GetComponent<UnitForage>(),
    //    };

    //    jobScript.OnIsAbleChanged += SetDefaultState;
    //    jobScript.OnIsMovingChanged += UpdateIsMoving;
    //}

    private void Start()
    {
        var unitWander = GetComponent<UnitWander>();
        unitWander.StartWander();
    }

    private void SetDefaultState()
    {
        if (currentState == UnitState.DIALOGUE) return;
        if (currentState == UnitState.FOLLOW) return;

        //if (doJob && jobScript.IsAble) SetCurrentState(UnitState.JOB);
    }

    private void SetCurrentState(UnitState state)
    {
        currentState = state;

        UpdateIsMoving();

        switch (state)
        {

            case UnitState.DIALOGUE:
                Vector3 targetPos = Camera.main.transform.position;
                targetPos.y = transform.position.y; // keep upright
                transform.LookAt(targetPos);
                break;
            case UnitState.FOLLOW:
                unitFollow.StartFollow(playerReference.Player.transform);
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
                    //agent.SetDestination(jobScript.TargetPosition);
                    break;

            }

            yield return null;
        }
    }

    private void UpdateIsMoving()
    {
        agent.isStopped = currentState switch
        {
            UnitState.DIALOGUE => true,
            //UnitState.JOB => !jobScript.IsMoving,
            _ => false,
        };

        //OnIsMovingHasChanged?.Invoke(!agent.isStopped);
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
        SetCurrentState(UnitState.ACTIVITY);
        agent.SetDestination(destination);
        transform.position = destination;
        transform.forward = direction;
    }

    public void SetTour()
    {
        SetCurrentState(UnitState.FOLLOW);
    }

    public void StopActivity()
    {
        SetDefaultState();
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
