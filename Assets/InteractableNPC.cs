using UnityEngine;
using UnityEngine.Events;

public class InteractableNPC : MonoBehaviour, IInteractable
{
    [SerializeField] private CameraPanController cameraPanController;
    public Transform Transform => transform;

    public event UnityAction OnInteractionStart;
    public event UnityAction OnInteractionComplete;

    public void OnBaseInteraction()
    {
        cameraPanController.CenterTargetInView(transform);
    }
}
