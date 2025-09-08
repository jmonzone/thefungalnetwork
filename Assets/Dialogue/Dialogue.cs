using System;
using UnityEngine;

public enum DialogueAction
{
    DEFAULT,
    SHOW_TAROT,
    PLAY_SPORE,
    FOLLOW,
    GIFT,
}

[Serializable]
public class Dialogue
{
    [SerializeField] [TextArea] private string text;
    [SerializeField] private DialogueAction action;

    public string Text => text;
    public DialogueAction Action => action;

    public Dialogue(string text, DialogueAction action = DialogueAction.DEFAULT)
    {
        this.text = text;
        this.action = action;
    }
}
