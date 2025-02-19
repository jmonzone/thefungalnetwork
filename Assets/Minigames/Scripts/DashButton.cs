using UnityEngine;

public class DashButton : Ability
{
    [SerializeField] private PlayerReference player;
    [SerializeField] private float castCooldown = 2f;
    [SerializeField] private MoveCharacterJoystick moveCharacterJoystick;

    // todo: maybe move character joystick is disabled by listening to the character
    // so that the move joystick handles itself, because dash isnt the only thing that will need to disable the josytick
    private void Movement_OnDestinationReached()
    {
        player.Movement.OnDestinationReached -= Movement_OnDestinationReached;
        moveCharacterJoystick.enabled = true;
    }

    public override void CastAbility(Vector3 direction)
    {
        moveCharacterJoystick.enabled = false;

        player.Movement.OnDestinationReached += Movement_OnDestinationReached;

        direction.y = 0; // Keep it in the XZ plane
        direction.Normalize(); // Normalize to maintain consistent speed

        var targetPosition = player.Movement.transform.position + direction.normalized * 3f;
        player.Movement.SetSpeed(7.5f);
        player.Movement.SetTargetPosition(targetPosition);
        cooldownHandler.StartCooldown(castCooldown); // Start logic cooldown
    }
}
