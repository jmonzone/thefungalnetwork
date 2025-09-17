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
    [SerializeField] private Dialogue next;
    [SerializeField] private List<Response> responses;
    [SerializeField] private DialogueType type;
    [SerializeField] private DialogueAction action;
    [SerializeField] private GlyphData glyph = null;

    public string Text => text;
    public Dialogue Next => next;
    public List<Response> Responses => responses;
    public DialogueType Type => type;
    public DialogueAction Action => action;
    public GlyphData Glyph => glyph;

    public Dialogue(string text, DialogueType type, DialogueAction action = DialogueAction.DEFAULT, GlyphData glyph = null)
    {
        this.text = text;
        responses = new List<Response>();
        this.type = type;
        this.action = action;
        this.glyph = glyph;
    }

    public Dialogue(List<Dialogue> dialogue, DialogueType type)
    {
        text = dialogue[0].text;
        action = dialogue[0].action;

        this.type = type;
        responses = new List<Response>();

        if (dialogue.Count > 1)
        {
            next = new Dialogue(dialogue.GetRange(1, dialogue.Count - 1), type);
        }
    }
}
