using System;
using System.Collections.Generic;
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
    [SerializeField] private Dialogue next;

    public string Text => text;
    public Dialogue Next => next;

    public void Initialize(string text, Dialogue next)
    {
        this.text = text;
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
