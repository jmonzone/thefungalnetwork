using UnityEngine;

public abstract class InteractableController : MonoBehaviour
{
    private InteractableCameraController cameraController;

    protected abstract UIReference Reference { get; }

    protected virtual void Awake()
    {
        cameraController = GetComponent<InteractableCameraController>();
        Reference.OnClose += OnClose;
    }

    public virtual void OnSelect()
    {
        cameraController.ActivateCamera();
        Reference.Show();
    }

    protected virtual void OnClose()
    {
        cameraController.DeactivateCamera();
    }
}

public class SpellbookController : InteractableController
{
    [SerializeField] private SpellbookReference spellbook;

    protected override UIReference Reference => spellbook;
}
