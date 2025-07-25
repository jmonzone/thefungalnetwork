using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    Transform Transform { get; }
    void OnBaseInteraction();
    event UnityAction OnInteractionStart;
    event UnityAction OnInteractionComplete;
}

public class InteractionController : MonoBehaviour
{
    public LayerMask groundMask;

    private Camera mainCamera;

    public event UnityAction<Vector3> OnInteractionStart;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 inputPos = Input.mousePosition;
            TryInteract(inputPos);
        }
    }

    void TryInteract(Vector3 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            IInteractable interactable = hit.transform.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                interactable.OnBaseInteraction();
                OnInteractionStart?.Invoke(interactable.Transform.position);
                return;
            }

        }

        if (Physics.Raycast(ray, out hit, 100f, groundMask))
        {
            OnInteractionStart?.Invoke(hit.point);
        }
    }
}
