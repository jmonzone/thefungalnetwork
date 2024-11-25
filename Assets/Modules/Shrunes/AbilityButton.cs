using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Controller controller;
    [SerializeField] private InventoryButton spellButton;

    private Vector3 mousePosition;

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        mousePosition = Input.mousePosition;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (spellButton.PreviewItem && spellButton.PreviewItem.Data is ShruneItem shrune)
        {
            controller.SetAnimation();

            // Get the direction from the mouse position relative to the screen space
            Vector3 mouseDirection = Input.mousePosition - mousePosition;
            mouseDirection.z = mouseDirection.y;
            mouseDirection.y = 0f; // Ignore vertical difference for XZ plane direction

            // Get the camera's forward rotation, but only around the Y axis (XZ plane)
            Quaternion cameraRotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);

            // Rotate the mouse direction by the camera's rotation
            Vector3 rotatedMouseDirection = cameraRotation * mouseDirection.normalized;

            var projectile = Instantiate(shrune.ProjectilePrefab, controller.Movement.transform.position + Vector3.up + rotatedMouseDirection.normalized * 1f, Quaternion.identity);
            
            // Apply the rotated direction to the projectile
            projectile.transform.rotation = Quaternion.LookRotation(rotatedMouseDirection);
            projectile.Shoot(rotatedMouseDirection);
        }
    }


}
