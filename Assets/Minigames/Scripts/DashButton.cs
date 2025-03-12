using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DashButton : Ability
{
    [SerializeField] private PlayerReference player;
    [SerializeField] private float castCooldown = 2f;
    [SerializeField] private MoveCharacterJoystick moveCharacterJoystick;
    [SerializeField] private List<AudioClip> audioClips;

    // todo: maybe move character joystick is disabled by listening to the character
    // so that the move joystick handles itself, because dash isnt the only thing that will need to disable the josytick
    private void Movement_OnDestinationReached()
    {
        player.Movement.OnDestinationReached -= Movement_OnDestinationReached;
        moveCharacterJoystick.enabled = true;
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        moveCharacterJoystick.enabled = false;

        player.Movement.OnDestinationReached += Movement_OnDestinationReached;

        player.Movement.SetSpeed(7.5f);
        player.Movement.SetTargetPosition(targetPosition);

        var audioClip = audioClips.GetRandomItem();
        player.Fungal.PlayAudioClip(audioClip, 1.5f);
        cooldownHandler.StartCooldown(castCooldown); // Start logic cooldown
    }

   

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