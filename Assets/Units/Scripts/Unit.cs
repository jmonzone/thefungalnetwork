using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Unit : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject prefab;
    [SerializeField] [TextArea] private List<string> intros; 
    [SerializeField] private DialogueTree dialogueTree;

    public string Name => name;
    public Sprite Sprite => sprite;
    public GameObject Prefab => prefab;
    public List<string> Intros => intros;
    public DialogueTree DialogueTree => dialogueTree;

}
