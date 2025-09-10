using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnitInstance
{
    [SerializeField] private Unit unit;
    [SerializeField] private bool isHired;
    [SerializeField] private float relationship;

    public Unit Data => unit;
    public bool IsHired => isHired;
    public float Relationship => relationship;

    public event UnityAction<float> OnRelationshipChanged;

    public UnitInstance(Unit unit, bool isHired, float relationship)
    {
        this.unit = unit;
        this.isHired = isHired;
        this.relationship = relationship;
    }

    public void IncreaseRelationship(float value)
    {
        relationship += value;
        OnRelationshipChanged?.Invoke(value);
    }
}

[CreateAssetMenu]
public class Unit : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject prefab;
    [SerializeField] [TextArea] private List<string> intros;
    [SerializeField] private List<Dialogue> chatDialogue;
    [SerializeField] private List<Dialogue> giveDialogue;

    public string Name => name;
    public Sprite Sprite => sprite;
    public GameObject Prefab => prefab;
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
            var chat = BuildDialogueTree(chatArray[0] as JObject, chatArray);
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

                    Dialogue d = new Dialogue(text, action);
                    giveDialogue.Add(d);
                }
                else
                {
                    Dialogue d = new Dialogue(g.ToString());
                    giveDialogue.Add(d);
                }
            }
        }
    }

    private Dialogue BuildDialogueTree(JObject lineObj,JArray chatArray)
    {
        string text = lineObj.Value<string>("text");
        var dialogue = new Dialogue(text);

        if (lineObj["responses"] is JArray responses)
        {
            foreach (var resp in responses)
            {
                string respText = resp.Value<string>("text");
                string nextId = resp.Value<string>("nextId");
                float xp = resp.Value<float>("xp");

                var response = CreateInstance<Response>();
                response.Initialize(respText, xp);

                if (!string.IsNullOrEmpty(nextId))
                {
                    // find the object in chatArray with this id
                    var nextLine = chatArray.First(l => l.Value<string>("id") == nextId) as JObject;
                    var childDialogue = BuildDialogueTree(nextLine, chatArray);

                    response.SetNext(childDialogue);
                }

                dialogue.Responses.Add(response);

            }
        }

        return dialogue;
    }

}
