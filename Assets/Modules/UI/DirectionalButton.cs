using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// this script is used to provide events for a directional button
public class DirectionalButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private Button button;
    [SerializeField] private float sensitivity = 0.01f;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image outline;
    [SerializeField] private GameObject background;

    private Vector3 initalPosition;
    private Vector3 direction;
    public bool DragStarted { get; private set; }

    public event UnityAction OnPointerDown;
    public event UnityAction OnPointerUp;

    public event UnityAction OnDragStarted;
    public event UnityAction OnDragCanceled;
    public event UnityAction<Vector3> OnDragUpdated;
    public event UnityAction<Vector3> OnDragCompleted;

    private void Awake()
    {
        //button.onClick.AddListener(() =>
        //{
        //    OnClick?.Invoke();
        //});
    }

    private void Update()
    {
        if (DragStarted) OnDragUpdated?.Invoke(direction);
    }

    private void OnEnable()
    {
        button.enabled = true;
        outline.gameObject.SetActive(true);
        canvasGroup.alpha = 1;
    }

    private void OnDisable()
    {
        button.enabled = false;
        outline.gameObject.SetActive(false);
        canvasGroup.alpha = 0.25f;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        DragStarted = true;
        background.SetActive(true);

        // Start casting ability
        OnDragStarted?.Invoke();

        // Record the initial touch position
        initalPosition = eventData.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!DragStarted) return;

        // Calculate the direction from the initial touch position
        Vector3 dragDirection = (Vector3)eventData.position - initalPosition;

        // Convert to XZ plane direction
        dragDirection.z = dragDirection.y;
        dragDirection.y = 0f; // Ignore vertical difference for XZ plane direction

        // Get the camera's forward rotation, but only around the Y axis (XZ plane)
        Quaternion cameraRotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);

        // Rotate the drag direction by the camera's rotation
        direction = cameraRotation * dragDirection * sensitivity;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!DragStarted) return;
        DragStarted = false;
        background.SetActive(false);

        OnDragCompleted?.Invoke(direction);

        //if (IsPointerOverThisButton(eventData))
        //{
        //    OnDragCanceled?.Invoke();
        //}
        //else
        //{
        //    OnDragCompleted?.Invoke(direction);
        //}
    }

    /// <summary>
    /// Checks if the pointer is still over this button.
    /// </summary>
    private bool IsPointerOverThisButton(PointerEventData eventData)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = eventData.position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == button.targetGraphic.gameObject) return true;
        }

        return false;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (DragStarted) return;
        OnPointerUp?.Invoke();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
        OnPointerDown?.Invoke();
    }
}
