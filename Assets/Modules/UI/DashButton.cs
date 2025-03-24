using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.VisualScripting;
using UnityEngine;

public class DashButton : Ability
{
    [SerializeField] private PlayerReference player;
    [SerializeField] private float castCooldown = 2f;
    [SerializeField] private MoveCharacterJoystick moveCharacterJoystick;
    [SerializeField] private List<AudioClip> audioClips;

    public override void CastAbility(Vector3 targetPosition)
    {
        moveCharacterJoystick.enabled = false;

        player.Fungal.Dash(targetPosition, () =>
        {
            moveCharacterJoystick.enabled = true;
        });

        cooldownHandler.StartCooldown(castCooldown); // Start logic cooldown
    }

    public override Vector3 DefaultTargetPosition => player.Fungal.transform.position + player.Fungal.transform.forward * range;
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