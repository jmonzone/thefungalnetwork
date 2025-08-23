using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDialogue : MonoBehaviour
{
    [SerializeField] private DialogueReference dialogue;
    private UnitController unit;

    private void Awake()
    {
        unit = GetComponent<UnitController>();
        unit.OnSelected += Unit_OnSelected;
    }

    private void Unit_OnSelected()
    {
        dialogue.ShowDialogue(unit.Data);
    }
}
