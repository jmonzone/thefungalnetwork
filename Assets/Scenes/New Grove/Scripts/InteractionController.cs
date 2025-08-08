using UnityEngine;
using UnityEngine.Events;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private UnitController selectedUnit;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private CameraPanController cameraPanController;

    private Camera mainCamera;

    private Vector3 startInput;
    private bool isDragging = false;


    public event UnityAction<Transform> OnEntitySelected;
    public event UnityAction<Vector3> OnGroundSelected;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
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

            if (selectedUnit)
            {
                var forage = selectedUnit.GetComponent<UnitForage>();
                if (forage)
                {
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000f, interactableMask))
                    {
                        var forageable = hit.transform.GetComponentInParent<Forageable>();
                        if (forageable)
                        {
                            OnEntitySelected?.Invoke(forageable.transform);
                            forage.StartForage(forageable);
                            return;
                        }
                    }
                }


                var movement = selectedUnit.GetComponent<UnitMovement>();
                if (movement)
                {
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundMask))
                    {
                        movement.StartMovement(hit.point);
                        OnGroundSelected?.Invoke(hit.point);
                        return;
                    }
                }
            }
            else
            {
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f, interactableMask))
                {
                    var unit = hit.transform.GetComponentInParent<UnitController>();
                    if (unit)
                    {
                        cameraPanController.CenterTargetInView(unit.transform);
                        unit.Select();
                        OnEntitySelected?.Invoke(unit.transform);
                        selectedUnit = unit;
}
                }
            }
        }
    }
}
