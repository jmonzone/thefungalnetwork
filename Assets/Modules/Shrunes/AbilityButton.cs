using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Controller controller;
    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private InventoryButton spellButton;

    private Vector3 mousePosition;

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (spellButton.PreviewItem && spellButton.PreviewItem.Data is ShruneItem shrune)
        {
            abilityCast.StartCast(controller.Movement.transform, shrune.MaxDistance);
            mousePosition = Input.mousePosition;
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (spellButton.PreviewItem && spellButton.PreviewItem.Data is ShruneItem)
        {
            // Get the direction from the mouse position relative to the screen space
            Vector3 mouseDirection = Input.mousePosition - mousePosition;
            mouseDirection.z = mouseDirection.y;
            mouseDirection.y = 0f; // Ignore vertical difference for XZ plane direction

            // Get the camera's forward rotation, but only around the Y axis (XZ plane)
            Quaternion cameraRotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);

            // Rotate the mouse direction by the camera's rotation
            Vector3 rotatedMouseDirection = cameraRotation * mouseDirection.normalized;

            abilityCast.UpdateCast(rotatedMouseDirection);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (spellButton.PreviewItem && spellButton.PreviewItem.Data is ShruneItem shrune)
        {
            controller.SetAnimation();
            abilityCast.EndCast();

            var direction = abilityCast.Direction;

            var projectile = Instantiate(shrune.ProjectilePrefab, controller.Movement.transform.position + Vector3.up + direction.normalized * 1f, Quaternion.identity);
            
            // Apply the rotated direction to the projectile
            projectile.transform.rotation = Quaternion.LookRotation(direction);
            projectile.Shoot(direction, shrune.MaxDistance);
        }
    }


}
