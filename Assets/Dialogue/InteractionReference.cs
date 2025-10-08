using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InteractionReference : ScriptableObject
{
    [SerializeField] private DialogueReference dialogueReference;

    public void StartInteraction(PlayerController player, IInteractable interactable)
    {
        if (interactable is UnitController unit)
        {
            if (unit.Dialogue.IsActive)
            {
                var units = new List<UnitController>
                {
                    player,
                    unit,
                };

                foreach(var target in unit.Dialogue.Targets)
                {
                    units.Add(target);
                }

                var origin = GetMidpoint(unit);

                dialogueReference.StartGroupDialogue(origin, unit, units);
            }
            else
            {
                dialogueReference.StartIntroDialogue(unit);
            }
        }
    }

    Vector3 GetMidpoint(UnitController unit)
    {
        if (unit == null) return Vector3.zero;

        var positions = new List<Vector3> { unit.transform.position };

        foreach (var target in unit.Dialogue.Targets)
        {
            if (target != null)
                positions.Add(target.transform.position);
        }

        if (positions.Count == 0)
            return Vector3.zero;

        // Average all positions
        Vector3 sum = Vector3.zero;
        foreach (var pos in positions)
            sum += pos;

        return sum / positions.Count;
    }


}
