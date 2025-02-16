using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// this script is used to provide events for a directional button
public class DirectionalButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject render;
    [SerializeField] private Image image;

    private Vector3 initialTouchPosition;
    private bool castStarted = false;

    public event UnityAction OnClick;
    public event UnityAction OnDragStarted;
    public event UnityAction<Vector3> OnDragUpdated;
    public event UnityAction OnDragCompleted;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            OnClick?.Invoke();
        });
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        castStarted = true;

        // Start casting ability
        OnDragStarted?.Invoke();

        // Record the initial touch position
        initialTouchPosition = eventData.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!castStarted) return;

        // Calculate the direction from the initial touch position
        Vector3 dragDirection = (Vector3)eventData.position - initialTouchPosition;

        // Convert to XZ plane direction
        dragDirection.z = dragDirection.y;
        dragDirection.y = 0f; // Ignore vertical difference for XZ plane direction

        // Get the camera's forward rotation, but only around the Y axis (XZ plane)
        Quaternion cameraRotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);

        // Rotate the drag direction by the camera's rotation
        Vector3 rotatedDragDirection = cameraRotation * dragDirection.normalized;

        OnDragUpdated?.Invoke(rotatedDragDirection);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        castStarted = false;
        OnDragCompleted?.Invoke();
    }
}
