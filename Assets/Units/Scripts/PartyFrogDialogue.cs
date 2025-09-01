using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PartyFrogDialogue : DialogueTree
{
    [SerializeField] private BuildReference buildReference;
    [SerializeField] private Item djTable;

    [SerializeField] private List<Dialogue> tip1;

    public override List<Dialogue> Dialogue
    {
        get
        {
            if (buildReference.Contains(djTable)) return tip1;
            else return base.Dialogue;
        }
    }
}