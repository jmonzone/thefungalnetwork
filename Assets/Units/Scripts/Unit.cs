using System;
using System.Collections.Generic;
using UnityEngine;

public enum DialogueAction
{
    DEFAULT,
    SHOW_TAROT,
}

[Serializable]
public class Dialogue
{
    [SerializeField] [TextArea] private string text;
    [SerializeField] private DialogueAction action;

    public string Text => text;
    public DialogueAction Action => action;
}

[CreateAssetMenu]
public class Unit : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<Dialogue> dialogueList;

    public string Name => name;
    public Sprite Sprite => sprite;
    public GameObject Prefab => prefab;
    public List<Dialogue> DialogueList => dialogueList;

}
