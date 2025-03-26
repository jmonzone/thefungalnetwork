using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FungalDash : Ability
{
    public event UnityAction OnDashStart;
    public event UnityAction OnDashComplete;

    public override void CastAbility(Vector3 targetPosition)
    {
        Debug.Log($"Dash {gameObject.name}");
        OnDashStart?.Invoke();
        //if (moveCharacterJoystick) moveCharacterJoystick.enabled = false;

        networkFungal.Dash(targetPosition, () =>
        {
            Debug.Log($"Dash complete");
            OnDashComplete?.Invoke();
            //if (moveCharacterJoystick) moveCharacterJoystick.enabled = true;
        });
    }

    public override Vector3 DefaultTargetPosition => transform.position + transform.forward * range;
}

public static class Extensions
{
    public static T GetRandomItem<T>(this List<T> items)
    {
        if (items.Count == 0) return default;

        var randomIndex = Random.Range(0, items.Count);
        return items[randomIndex];
    }
}