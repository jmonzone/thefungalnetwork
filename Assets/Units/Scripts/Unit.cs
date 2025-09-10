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
    [SerializeField] private int relationshipLevel;
    [SerializeField] private float relationshipPoints;

    public Unit Data => unit;
    public bool IsFriends => relationshipLevel > 1;
    public int RelationshipLevel => relationshipLevel;
    public float RelationshipPoints => relationshipPoints;
    public float MinimumRelationshipPoints => GetXPFromLevel(relationshipLevel);
    public float MaximumRelationshipPoints => GetXPFromLevel(relationshipLevel + 1);
    public float RelationshipPointsUntilNextLevel => MaximumRelationshipPoints - relationshipPoints;
    public event UnityAction<float> OnRelationshipChanged;
    public event UnityAction OnRelationshipLevelChanged;

    public UnitInstance(Unit unit, float relationshipPoints)
    {
        this.unit = unit;
        this.relationshipPoints = relationshipPoints;
        relationshipLevel = GetLevelFromXP(relationshipPoints);
    }

    public void IncreaseRelationship(float value)
    {
        SetRelationshipPoints(relationshipPoints + value);
        OnRelationshipChanged?.Invoke(value);
    }

    private void SetRelationshipPoints(float value)
    {
        relationshipPoints = value;

        var previousLevel = relationshipLevel;
        relationshipLevel = GetLevelFromXP(relationshipPoints);

        if (previousLevel != relationshipLevel) OnRelationshipLevelChanged?.Invoke();
    }

    public int GetLevelFromXP(float xp)
    {
        int level = 1;
        double points = 0;

        for (int lvl = 1; lvl <= 120; lvl++) // RuneScape goes to 99/120, you can adjust cap
        {
            points += Math.Floor(lvl + 300 * Math.Pow(2, lvl / 7.0));
            double output = Math.Floor(points / (4 * 10));

            if (output > xp)
            {
                level = lvl;
                break;
            }
        }

        return level;
    }

    public static int GetXPFromLevel(int level)
    {
        double points = 0;

        for (int lvl = 1; lvl < level; lvl++)
        {
            points += Math.Floor(lvl + 300 * Math.Pow(2, lvl / 7.0));
        }

        return (int)Math.Floor(points / (4 * 10));
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
