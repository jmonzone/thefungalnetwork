using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

[CreateAssetMenu]
public class Unit : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject prefab;

    [Tooltip("Column mapping for this Unit. -1 = keep original, 0–7 = palette index.")]
    [SerializeField] private int[] columnMapping = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };

    [SerializeField] private List<Dialogue> chatDialogue;
    [SerializeField] private List<DanceMove> moves;

    public string Name => name;
    public Sprite Sprite => sprite;
    public GameObject Prefab => prefab;
    public List<DanceMove> Moves => moves;

    public int[] ColumnMapping => columnMapping;

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
            var dialogue = new Dialogue(lineToken.ToString(), element.ToString());
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
