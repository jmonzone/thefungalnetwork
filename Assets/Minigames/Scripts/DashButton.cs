using UnityEngine;

public class DashButton : MonoBehaviour
{
    [SerializeField] private DirectionalButton directionalButton;
    [SerializeField] private PlayerReference player;
    [SerializeField] private CooldownHandler cooldownHandler;
    [SerializeField] private AbilityCastIndicator abilityCastIndicator;
    [SerializeField] private float castCooldown = 2f;
    [SerializeField] private MoveCharacterJoystick moveCharacterJoystick;

    private Vector3 direction;

    private void Awake()
    {
        directionalButton.OnDragStarted += DirectionalButton_OnDragStarted;
        directionalButton.OnDragUpdated += DirectionalButton_OnDragUpdated;
        directionalButton.OnDragCompleted += DirectionalButton_OnDragCompleted;
    }

    private void Movement_OnDestinationReached()
    {
        player.Movement.OnDestinationReached -= Movement_OnDestinationReached;
        moveCharacterJoystick.enabled = true;
    }

    private void DirectionalButton_OnDragUpdated(Vector3 direction)
    {
        this.direction = direction;
    }

    private void Update()
    {
        if (directionalButton.CastStarted)
        {
            var clampedDirection = Vector3.ClampMagnitude(direction * 0.01f, 2f);
            abilityCastIndicator.UpdateIndicator(player.Movement.transform.position, clampedDirection);
        }
    }

    private void DirectionalButton_OnDragStarted()
    {
        if (cooldownHandler.IsOnCooldown) return;
        abilityCastIndicator.ShowIndicator();
    }

    private void DirectionalButton_OnDragCompleted(Vector3 direction)
    {
        if (cooldownHandler.IsOnCooldown) return;

        moveCharacterJoystick.enabled = false;

        player.Movement.OnDestinationReached += Movement_OnDestinationReached;

        direction.y = 0; // Keep it in the XZ plane
        direction.Normalize(); // Normalize to maintain consistent speed

        var targetPosition = player.Movement.transform.position + direction.normalized * 3f;
        player.Movement.SetSpeed(5f);
        player.Movement.SetTargetPosition(targetPosition);
        cooldownHandler.StartCooldown(castCooldown); // Start logic cooldown

        abilityCastIndicator.HideIndicator();
    }
}
