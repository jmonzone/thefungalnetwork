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

    private Interactable selectedInteractable;

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

            if (selectedInteractable)
            {
                var movement = selectedInteractable.GetComponent<CharacterMovement>();
                if (movement)
                {
                    Ray ray = mainCamera.ScreenPointToRay(inputPos);

                    if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
                    {
                        movement.MoveToPosition(hit.point);
                    }
                }
            }
            else
            {
                TryInteract(inputPos);
            }
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
                if (interactable is Interactable inter)
                {
                    selectedInteractable = inter;
                    //OnInteractableSelected?.Invoke(inter);
                }
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
