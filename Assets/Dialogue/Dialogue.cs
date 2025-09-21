using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public enum DialogueAction
{
    DEFAULT,
    SHOW_TAROT,
    PLAY_SPORE,
    FOLLOW,
    GIFT,
}

public class Response : ScriptableObject
{
    [SerializeField] [TextArea] private string text;
    [SerializeField] private float xp;
    [SerializeField] private float relationship;
    [SerializeField] private Dialogue next;

    public string Text => text;
    public float XP => xp;
    public float Relationship => relationship;
    public Dialogue Next => next;
    public bool HasNext => next != null;

    public void Initialize(JToken json)
    {
        text = json.Value<string>("text");
        xp = json.Value<float>("xp");
        relationship = json.Value<float>("relationship");
    }

    public void SetNext(Dialogue next)
    {
        this.next = next;
    }
}

public enum DialogueType
{
    CHAT,
    GIFT,
    FRIEND,
    STORY
}

[Serializable]
public class Dialogue
{
    [SerializeField] [TextArea] private string text;
    [SerializeField] private List<Response> responses;
    [SerializeField] private Dialogue next; // next line in the chain
    [SerializeField] private DialogueType type;
    [SerializeField] private DialogueAction action;
    [SerializeField] private string element;

    public string Text => text;
    public List<Response> Responses => responses;
    public Dialogue Next => next;
    public DialogueType Type => type;
    public DialogueAction Action => action;
    public string Element => element;

    // Single-line constructor
    public Dialogue(string text, string element = "neutral", DialogueType type = DialogueType.CHAT, DialogueAction action = DialogueAction.DEFAULT)
    {
        this.text = text;
        this.element = element;
        this.type = type;
        this.action = action;
        responses = new List<Response>();
    }

    // List-based constructor that chains dialogues
    public Dialogue(List<Dialogue> dialogueList, DialogueType type = DialogueType.CHAT)
    {
        if (dialogueList == null || dialogueList.Count == 0)
            throw new ArgumentException("Dialogue list cannot be null or empty");

        text = dialogueList[0].text;
        action = dialogueList[0].action;
        element = dialogueList[0].element;

        this.type = type;
        responses = new List<Response>();

        if (dialogueList.Count > 1)
        {
            // Recursively chain the remaining dialogues
            next = new Dialogue(dialogueList.GetRange(1, dialogueList.Count - 1), type);
        }
    }

    public void SetNext(Dialogue nextDialogue)
    {
        next = nextDialogue;
    }
}