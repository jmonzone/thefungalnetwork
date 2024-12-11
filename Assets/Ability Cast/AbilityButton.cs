using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// this script is used to trigger events on ability cast
public class AbilityButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Button button;
    [SerializeField] private Controller controller;
    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private Image preview;
    [SerializeField] private GameObject render;

    private Vector3 mousePosition;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            abilityCast.Cast(controller.Movement.transform, controller.Movement.transform.forward);
        });
    }

    private void OnEnable()
    {
        UpdatePreview();
        abilityCast.OnShruneChanged += UpdatePreview;
    }

    private void OnDisable()
    {
        abilityCast.OnShruneChanged -= UpdatePreview;

    }

    private void UpdatePreview()
    {
        render.SetActive(abilityCast.Shrune);

        if (abilityCast.Shrune)
        {
            preview.enabled = true;
            preview.sprite = abilityCast.Shrune.Sprite;
        }
        else
        {
            preview.enabled = false;
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        abilityCast.StartCast(controller.Movement.transform, attackable => attackable != controller.Attackable);
        mousePosition = Input.mousePosition;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
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

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        controller.SetAnimation();
        abilityCast.Cast();
    }


}
