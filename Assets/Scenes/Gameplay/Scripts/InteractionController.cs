using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    public Transform Transform { get; }
    public void Select();
}

public class InteractionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference homeView;
    [SerializeField] private ViewReference partyView;

    [Header("Runtime")]
    [SerializeField] private Transform selected;

    private Camera mainCamera;

    private Vector3 startInput;
    private bool isDragging = false;

    public event UnityAction<Transform> OnEntitySelected;
    public event UnityAction<Vector3> OnGroundSelected;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private float raycastMaxDistance = 100f;

    private void Update()
    {
        if (!homeView.Canvas.IsVisible && !partyView.Canvas.IsVisible) return;

        if (Input.GetMouseButtonDown(0))
        {
            startInput = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            var inputDelta = Input.mousePosition - startInput;
            if (inputDelta.magnitude > 0.1f) isDragging = true;
        }

        if (selected)
        {
            cameraPanController.CenterTargetInView(selected.position);
        }

        if (Input.GetMouseButtonUp(0) && !isDragging)
        {
            Vector3 inputPos = Input.mousePosition;

            Ray ray = mainCamera.ScreenPointToRay(inputPos);
            RaycastHit hit;

            if (Physics.SphereCast(ray, 0.5f, out hit, 1000f, interactableMask))
            {
                var interactable = hit.transform.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    playerReference.SetTargetInteractable(interactable);
                    OnEntitySelected?.Invoke(interactable.Transform);

                    selected = interactable.Transform;
                    return;
                }
            }

            if (Physics.Raycast(ray, out hit, raycastMaxDistance, groundMask))
            {
                cameraPanController.CenterTargetInView(hit.point);
                playerReference.SetTargetPosition(hit.point);
                OnGroundSelected?.Invoke(hit.point);

                selected = null;
            }
        }
    }
}
