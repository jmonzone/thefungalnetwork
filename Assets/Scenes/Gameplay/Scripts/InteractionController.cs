using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public interface ITarget
{
    public Transform Transform { get; }
}

public interface IInteractable : ITarget
{
    public void Select();
    public void OnProximityChanged(bool value);
}

public class InteractionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private LayerMask groundMask;
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
        dialogue.OnDialogueComplete += Unselect;
    }

    private float raycastMaxDistance = 100f;

    private List<IInteractable> previousInteractables = new List<IInteractable>();

    private void Update()
    {
        if (!homeView.Canvas.IsVisible) return;

        var proximityColliders = Physics.OverlapSphere(playerReference.Player.transform.position, 1f, interactableMask);

        var proximityInteractables = proximityColliders
            .Select(c => c.GetComponentInParent<IInteractable>())
            .Where(i => i != null)
            .ToList();

        // Handle leaving proximity
        foreach (var interactable in previousInteractables.Except(proximityInteractables).ToList())
        {
            interactable.OnProximityChanged(false);
            previousInteractables.Remove(interactable);
        }

        // Handle entering proximity
        foreach (var interactable in proximityInteractables.Except(previousInteractables).ToList())
        {
            interactable.OnProximityChanged(true);
            previousInteractables.Add(interactable);
        }


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

            if (Physics.SphereCast(ray, 0.001f, out hit, 1000f, interactableMask))
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
                playerReference.SetTargetPosition(hit.point);
                OnGroundSelected?.Invoke(hit.point);

                selected = null;
            }
        }
    }

    public void Unselect()
    {
        if (selected)
        {
            selected = null;
            OnEntitySelected?.Invoke(null);
        }
    }
}
