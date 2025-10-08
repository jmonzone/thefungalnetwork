using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GreetingDialogue
{
    [SerializeField] private UnitController unitA;
    [SerializeField] private UnitController unitB;
    [SerializeField] private Dialogue dialogue;

    public UnitController UnitA => unitA;
    public UnitController UnitB => unitB;
    public Dialogue Dialogue => dialogue;

    public GreetingDialogue(UnitController unitA, UnitController unitB)
    {
        this.unitA = unitA;
        this.unitB = unitB;

        dialogue = new Dialogue(new List<Dialogue>
        {
            new Dialogue(unitA.Instance.Data, "Hey this is my friend! Say hi :)"),
            new Dialogue(unitB.Instance.Data, "Hey it's great to meet you!"),
            new Dialogue(unitB.Instance.Data, "The party looks fun over here"),
        });
    }
}

public class UnitDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;

    [Header("Settings")]
    [SerializeField] private bool lookAtTarget = true;

    [Header("Runtime")]
    [SerializeField] private bool isActive = false;
    [SerializeField] private GreetingDialogue greetingDialogue;
    [SerializeField] private List<UnitController> targets;

    private UnitController unit;
    private Vector3 originalLookPosition;

    public bool IsActive => isActive;
    public GreetingDialogue GreetingDialogue => greetingDialogue;
    public List<UnitController> Targets => targets;

    public event UnityAction OnDialogueStarted;
    public event UnityAction OnDialogueCompleted;

    private void Awake()
    {
        unit = GetComponent<UnitController>();
        targets = new List<UnitController>();
    }

    public void StartGreetDialogue(GreetingDialogue greetingDialogue, UnitController target)
    {
        this.greetingDialogue = greetingDialogue;
        StartDialogue(target);
    }

    public void StartDialogue(UnitController target)
    {
        isActive = true;
        targets.Add(target);

        if (lookAtTarget)
        {
            originalLookPosition = unit.LookPosition;
            unit.SetLookTarget(target.transform);
        }

        OnDialogueStarted?.Invoke();
    }

    public void StartGroupDialogue(Vector3 origin)
    {
        isActive = true;

        if (lookAtTarget)
        {
            originalLookPosition = unit.LookPosition;
            unit.SetLookPosition(origin);
        }

        OnDialogueStarted?.Invoke();
    }

    public void CompleteDialogue()
    {
        isActive = false;
        greetingDialogue = null;
        targets = new List<UnitController>();

        unit.SetLookPosition(originalLookPosition);
        OnDialogueCompleted?.Invoke();
    }
}
