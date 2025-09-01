using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DialogueTree : ScriptableObject
{
    [SerializeField] private List<Dialogue> defaultDialogue;

    public virtual List<Dialogue> Dialogue => defaultDialogue;
}
