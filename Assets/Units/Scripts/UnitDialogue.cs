using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
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
            new Dialogue(unitA, "Hey this is my friend! Say hi :)"),
            new Dialogue(unitB, "Hey it's great to meet you!"),
            new Dialogue(unitB, "The party looks fun over here"),
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

    public List<Dialogue> Dialogue => ElementalDialogue[unit.Instance.Element];
    public Dialogue InviteAFriendDialogue => new Dialogue(unit, "I just invited my friend, they should be coming by here soon.");
    public Dialogue RandomDialogue => Dialogue[UnityEngine.Random.Range(0, Dialogue.Count)];

    public Dictionary<Element, List<Dialogue>> ElementalDialogue { get; private set; } = new();

    public void Initialize(JObject data)
    {
        ElementalDialogue.Clear();

        foreach (var elementProp in data.Properties())
        {
            if (!Enum.TryParse(elementProp.Name, true, out Element element))
                element = Element.NONE;

            if (elementProp.Value is not JObject elementObj)
                continue;

            // Reusable helper
            AddDialogueGroup(elementObj, "lines", ElementalDialogue, element);
        }
    }

    private void AddDialogueGroup(JObject elementObj, string key, Dictionary<Element, List<Dialogue>> targetDict, Element element)
    {
        if (elementObj[key] is not JArray groupArray)
            return;

        var dialogues = new List<Dialogue>();

        foreach (JArray lineGroup in groupArray)
        {
            Dialogue head = BuildDialogueChain(lineGroup, element);
            if (head != null)
                dialogues.Add(head);
        }

        targetDict[element] = dialogues;
    }

    private Dialogue BuildDialogueChain(JArray lineGroup, Element element)
    {
        Dialogue first = null;
        Dialogue previous = null;

        foreach (var lineToken in lineGroup)
        {
            var dialogue = new Dialogue(unit, lineToken.ToString(), element.ToString());
            if (first == null)
                first = dialogue;

            previous?.SetNext(dialogue);
            previous = dialogue;
        }

        return first;
    }

    // Get random dialogue chain for element
    public Dialogue GetDialogue(Element element)
    {
        if (ElementalDialogue.TryGetValue(element, out var list) && list.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }
        return null;
    }
}
