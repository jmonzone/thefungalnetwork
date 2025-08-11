using UnityEngine;
using UnityEngine.Events;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private Transform selected;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private CameraPanController cameraPanController;

    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference homeView;

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
        if (navigation.CurrentView != homeView) return;

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

        if (Input.GetMouseButtonUp(0) && !isDragging)
        {
            Vector3 inputPos = Input.mousePosition;

            Ray ray = mainCamera.ScreenPointToRay(inputPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, interactableMask))
            {
                var interactable = hit.transform.GetComponentInParent<InteractableController>();
                if (interactable && selected != interactable.transform)
                {
                    cameraPanController.CenterTargetInView(interactable.transform.position);
                    interactable.OnSelect();
                    selected = interactable.transform;
                    OnEntitySelected?.Invoke(null);
                    return;
                }

                var unit = hit.transform.GetComponentInParent<UnitController>();
                if (unit && selected != unit.transform)
                {
                    cameraPanController.CenterTargetInView(unit.transform.position);
                    unit.Select();
                    OnEntitySelected?.Invoke(unit.transform);
                    selected = unit.transform;
                    return;
                }
            }

            if (selected)
            {
                var forage = selected.GetComponent<UnitForage>();
                if (forage)
                {
                    if (Physics.Raycast(ray, out hit, raycastMaxDistance, interactableMask))
                    {
                        var forageable = hit.transform.GetComponentInParent<Forageable>();
                        if (forageable)
                        {
                            cameraPanController.CenterTargetInView(forageable.transform.position);
                            OnEntitySelected?.Invoke(forageable.transform);
                            forage.StartForage(forageable);
                            return;
                        }
                    }
                }
            }

            if (Physics.Raycast(ray, out hit, raycastMaxDistance, groundMask))
            {
                cameraPanController.CenterTargetInView(hit.point);
                OnGroundSelected?.Invoke(hit.point);

                if (selected)
                {
                    var movement = selected.GetComponent<UnitMovement>();
                    if (movement)
                    {
                        movement.StartMovement(hit.point);
                    }
                    return;
                }
            }
        }
    }
}
