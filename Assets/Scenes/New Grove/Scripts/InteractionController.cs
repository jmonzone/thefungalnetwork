using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    Transform Transform { get; }
    void OnBaseInteraction();
}

public class InteractionController : MonoBehaviour
{
    public LayerMask groundMask;

    private Camera mainCamera;

    public event UnityAction<Interactable> OnInteractableSelected;
    public event UnityAction<Vector3> OnGroundSelected;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 inputPos = Input.mousePosition;
            TryInteract(inputPos);
        }
    }

    private void TryInteract(Vector3 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var interactables = hit.transform.GetComponentsInParent<IInteractable>();

            foreach(var interactable in interactables)
            {
                if (interactable is Interactable inter) OnInteractableSelected?.Invoke(inter);
                interactable.OnBaseInteraction();
            }

            if (interactables.Length > 0) return;
        }

        if (Physics.Raycast(ray, out hit, 100f, groundMask))
        {
            OnGroundSelected?.Invoke(hit.point);
        }
    }
}
