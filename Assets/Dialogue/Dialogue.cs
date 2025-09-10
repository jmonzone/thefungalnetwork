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

[Serializable]
public class Dialogue
{
    [SerializeField] [TextArea] private string text;
    [SerializeField] private List<Response> responses;
    [SerializeField] private DialogueAction action;

    public string Text => text;
    public List<Response> Responses => responses;
    public DialogueAction Action => action;

    public Dialogue(string text, DialogueAction action = DialogueAction.DEFAULT)
    {
        this.text = text;
        this.action = action;
        responses = new List<Response>();
    }
}
