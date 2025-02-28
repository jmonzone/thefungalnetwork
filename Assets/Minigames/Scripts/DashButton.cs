using UnityEngine;

public class DashButton : Ability
{
    [SerializeField] private PlayerReference player;
    [SerializeField] private float castCooldown = 2f;
    [SerializeField] private MoveCharacterJoystick moveCharacterJoystick;
    [SerializeField] private LayerMask obstacleLayer;

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

        // Check for obstacles
        targetPosition = GetValidTargetPosition(targetPosition);

        player.Movement.OnDestinationReached += Movement_OnDestinationReached;

        player.Movement.SetSpeed(7.5f);
        player.Movement.SetTargetPosition(targetPosition);
        cooldownHandler.StartCooldown(castCooldown); // Start logic cooldown
    }

    /// <summary>
    /// Performs a raycast and clamps the target position if an obstacle is in the way.
    /// </summary>
    private Vector3 GetValidTargetPosition(Vector3 targetPosition)
    {
        Vector3 origin = player.Movement.transform.position;
        Vector3 direction = (targetPosition - origin).normalized;
        float maxDistance = Vector3.Distance(origin, targetPosition);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, obstacleLayer))
        {
            // If an obstacle is hit, adjust the target position to be just before the obstacle
            return hit.point - direction * 0.1f; // Slightly offset from the obstacle
        }

        return targetPosition; // No obstacle, use original position
    }

}
