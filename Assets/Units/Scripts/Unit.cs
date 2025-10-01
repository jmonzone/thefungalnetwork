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

    // Key: element, Value: list of dialogue chains
    public Dictionary<Element, List<Dialogue>> ElementalDialogue = new Dictionary<Element, List<Dialogue>>();

    public void Initialize(JObject data)
    {
        ElementalDialogue.Clear();

        foreach (var elementProp in data.Properties())
        {
            if (!Enum.TryParse<Element>(elementProp.Name, true, out var element))
                element = Element.NONE;

            if (elementProp.Value is JObject elementObj && elementObj["lines"] is JArray linesArray)
            {
                var dialogues = new List<Dialogue>();

                foreach (JArray lineGroup in linesArray)
                {
                    Dialogue first = null;
                    Dialogue previous = null;

                    foreach (var lineToken in lineGroup)
                    {
                        string text = lineToken.ToString();
                        var dialogue = new Dialogue(text, element.ToString());

                        if (first == null)
                            first = dialogue;

                        if (previous != null)
                            previous.SetNext(dialogue);

                        previous = dialogue;
                    }

                    if (first != null)
                        dialogues.Add(first); // add the head of the chain
                }

                ElementalDialogue[element] = dialogues;
            }
        }
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
