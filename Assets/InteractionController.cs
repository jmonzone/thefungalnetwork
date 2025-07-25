using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    void OnBaseInteraction();
    event UnityAction OnInteractionStart;
    event UnityAction OnInteractionComplete;
}

public class InteractionController : MonoBehaviour
{
    private Camera mainCamera;

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
                return;
            }
        }
    }
}
