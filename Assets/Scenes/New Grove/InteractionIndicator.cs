using UnityEngine;

public class InteractionIndicator : MonoBehaviour
{
    [Header("Rotation speed in degrees per second")]
    public float rotationSpeedY = 20f;

    [Tooltip("Toggle this to simulate interactable element state")]
    public IInteractable interactable;

    void Start()
    {
        interactable = GetComponentInParent<IInteractable>();
        interactable.OnInteractionComplete += Interactable_OnInteracted;
    }

    private void Interactable_OnInteracted()
    {
        gameObject.SetActive(false);
        interactable.OnInteractionStart -= Interactable_OnInteracted;
    }

    void Update()
    {
        transform.Rotate(0f, rotationSpeedY * Time.deltaTime, 0f);
    }
}
