using System.Collections.Generic;
using Cinemachine;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UnitController : MonoBehaviour, IInteractable
{
    [Header("Unit References")]
    [SerializeField] protected Transform renderRoot;
    [SerializeField] private UnitBehaviour defaultBehaviour;
    [SerializeField] private UnitDestination destination;
    [SerializeField] private UnitDialogue dialogue;

    public UnitDestination Destination => destination;
    public UnitDialogue Dialogue => dialogue;

    [Header("Runtime")]
    [SerializeField] private UnitInstance instance;
    [SerializeField] private UnitBehaviour currentBehaviour;
    [SerializeField] private Vector3 targetLookPosition;
    [SerializeField] private Transform target;

    public UnitInstance Instance => instance;
    public UnitBehaviour CurrentBehaviour => currentBehaviour;

    public Vector3 LookPosition => targetLookPosition;

    public bool IsDefaultBehaviour => currentBehaviour == defaultBehaviour;


    public virtual Color Color { get; set; }

    Transform ITarget.Transform => transform;

    public Transform RenderRoot => renderRoot;

    private CinemachineVirtualCamera virtualCamera;

    public event UnityAction OnInitialized;
    public event UnityAction OnBehaviourChanged;

    protected virtual void Awake()
    {
        targetLookPosition = transform.position;

        var allBehaviours = GetComponents<UnitBehaviour>();
        foreach(var behaviour in allBehaviours)
        {
            behaviour.OnBehaviourRequest += () => ApplyBehaviour(behaviour);
        }

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        destination = GetComponent<UnitDestination>();
        dialogue = GetComponent<UnitDialogue>();

        if (destination)
        {
            destination.OnTargetSelected += () => SetLookTarget(destination.Target);
        }


        if (dialogue)
        {
            dialogue.OnDialogueStarted += () => currentBehaviour.PauseBehaviour();
            dialogue.OnDialogueCompleted += () => currentBehaviour.UnpauseBehaviour();
        }
    }

    protected virtual void Start()
    {
        if (!currentBehaviour) ApplyDefaultBehaviour();
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void Update()
    {
        if (target) targetLookPosition = target.transform.position;

        Vector3 direction = targetLookPosition - renderRoot.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f) // make sure it's not zero
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            renderRoot.rotation = Quaternion.Slerp(
                renderRoot.rotation,
                targetRotation,
                Time.deltaTime * 5f // rotation speed factor
            );
        }
    }

    protected void ApplyBehaviour(UnitBehaviour behaviour)
    {
        if (currentBehaviour)
        {
            currentBehaviour.OnBehaviourComplete -= ApplyDefaultBehaviour;
            currentBehaviour.StopBehaviour();
        }

        currentBehaviour = behaviour;

        if (currentBehaviour)
        {
            currentBehaviour.OnBehaviourComplete += ApplyDefaultBehaviour;
            currentBehaviour.StartBehaviour();
        }

        OnBehaviourChanged?.Invoke();
    }

    public virtual void Initialize(UnitInstance instance)
    {
        this.instance = instance;
        name = "Unit - " + instance.Data.Name;

        OnInitialized?.Invoke();
    }

    public void SetLookPosition(Vector3 targetPosition)
    {
        target = null;
        targetLookPosition = targetPosition;
    }

    public void SetLookTarget(Transform target)
    {
        this.target = target;
    }

    public void SetDefaultBehaviour(UnitBehaviour behaviour)
    {
        defaultBehaviour = behaviour;
        ApplyDefaultBehaviour();
    }

    public void ApplyDefaultBehaviour()
    {
        ApplyBehaviour(defaultBehaviour);
    }

    public void SetBehaviour(UnitBehaviour behaviour)
    {
        ApplyBehaviour(behaviour);
    }

    public void Select()
    {

    }

    public virtual void OnProximityChanged(bool value)
    {
    }

    public void Focus()
    {
        if (virtualCamera) virtualCamera.Priority = 11;
    }

    public void Unfocus()
    {
        if (virtualCamera) virtualCamera.Priority = 0;
    }

    public static void ArrangeUnitsInRadius(Vector3 origin, List<UnitController> units)
    {
        int count = units.Count;
        if (count == 0) return;

        if (count == 1)
        {
            units[0].Destination.SetDestination(origin);
            units[0].SetLookPosition(origin); // face the center
            return;
        }

        float radius = 1.0f; // radius of the circle
        float offset = Random.Range(0f, Mathf.PI * 2f); // random rotation of the circle

        for (int i = 0; i < count; i++)
        {
            // Evenly spaced angle around the circle
            float angle = -(i / (float)count) * Mathf.PI * 2f + offset;

            // Compute position on circle
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            Vector3 destination = origin + direction;

            // Update unit
            units[i].Destination.SetDestination(destination);
            units[i].SetLookPosition(origin); // face the center
        }
    }
}
