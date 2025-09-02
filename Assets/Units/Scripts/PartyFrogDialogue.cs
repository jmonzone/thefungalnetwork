using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueSet
{
    [SerializeField] private List<Dialogue> dialogue;

    public List<Dialogue> Dialogue => dialogue;
}

[CreateAssetMenu]
public class PartyFrogDialogue : DialogueTree
{
    [SerializeField] private BuildReference buildReference;
    [SerializeField] private Item djTable;

    [SerializeField] private List<DialogueSet> allDJDialogue;

    public override List<Dialogue> Dialogue
    {
        get
        {
            if (buildReference.Contains(djTable))
            {
                return GetRandomDialogue(allDJDialogue);
            }
            else
            {
                return base.Dialogue;
            }
        }
    }
}