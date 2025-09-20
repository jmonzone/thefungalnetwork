using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

[CreateAssetMenu]
public class Unit : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject prefab;

    [Header("Palette")]
    [Tooltip("Column mapping for this Unit. -1 = keep original, 0–7 = palette index.")]
    [SerializeField] private int[] columnMapping = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };

    [Header("Dialogue")]
    [SerializeField] [TextArea] private List<string> intros;
    [SerializeField] private List<Dialogue> chatDialogue;
    [SerializeField] private List<Dialogue> giveDialogue;

    public string Name => name;
    public Sprite Sprite => sprite;
    public GameObject Prefab => prefab;

    public int[] ColumnMapping => columnMapping;

    public List<string> Intros => intros;
    public List<Dialogue> GiveDialogue => giveDialogue;
    public List<Dialogue> ChatDialogue => chatDialogue;

    public void Initialize(JObject data)
    {
        // Greetings → intros
        intros = new List<string>();
        if (data["greetings"] is JArray greetingsArray)
        {
            foreach (var g in greetingsArray)
            {
                intros.Add(g.ToString());
            }
        }

        // Chat dialogue
        chatDialogue = new List<Dialogue>();
        if (data["chat"] is JArray chatArray)
        {
            var chat = BuildDialogueTree(chatArray[0] as JObject, chatArray, DialogueType.CHAT);
            chatDialogue.Add(chat);
        }

        // Gift dialogue
        giveDialogue = new List<Dialogue>();
        if (data["gift"] is JArray giftArray)
        {
            foreach (var g in giftArray)
            {
                if (g is JObject giftObj)
                {
                    var text = giftObj.Value<string>("text") ?? string.Empty;
                    var actionStr = giftObj.Value<string>("action");

                    var action = actionStr switch
                    {
                        "spore" => DialogueAction.PLAY_SPORE,
                        _ => DialogueAction.DEFAULT,
                    };

                    Dialogue d = new Dialogue(text, DialogueType.GIFT, action);
                    giveDialogue.Add(d);
                }
                else
                {
                    Dialogue d = new Dialogue(g.ToString(), DialogueType.GIFT);
                    giveDialogue.Add(d);
                }
            }
        }
    }

    private Dialogue BuildDialogueTree(JObject lineObj,JArray chatArray, DialogueType type)
    {
        string text = lineObj.Value<string>("text");

        var dialogue = new Dialogue(text, type);

        if (lineObj["responses"] is JArray responses)
        {
            foreach (var json in responses)
            {
                string nextId = json.Value<string>("nextId");

                var response = CreateInstance<Response>();
                response.Initialize(json);

                if (!string.IsNullOrEmpty(nextId))
                {
                    // find the object in chatArray with this id
                    var nextLine = chatArray.First(l => l.Value<string>("id") == nextId) as JObject;
                    var childDialogue = BuildDialogueTree(nextLine, chatArray, type);

                    response.SetNext(childDialogue);
                }

                dialogue.Responses.Add(response);

            }
        }

        return dialogue;
    }

}
