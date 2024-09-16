using UnityEngine;
using UnityEngine.Events;

public enum FungalState
{
    RANDOM,
    ESCORT,
    TARGET,
}

[RequireComponent(typeof(ProximityAction))]
public class FungalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MovementController movement;
    [SerializeField] private Camera spotlightCamera;

    public FungalModel Model { get; private set; }
    public GameObject Render { get; private set; }
    public MovementController Movement => movement;
    public bool IsFollowing { get; set; }

    public Camera SpotlightCamera => spotlightCamera;

    public event UnityAction OnInteractionStarted;

    public void Initialize(FungalModel model, Collider bounds)
    {
        Debug.Log($"initializing fungal controller {model}");

        Model = model;

        if (model)
        {
            name = $"Fungal Controller - {model.name}";
            Render = Instantiate(model.Data.Prefab, transform);
            Render.transform.localScale = Vector3.one;

            var proximityAction = GetComponent<ProximityAction>();
            proximityAction.OnUse += StartInteraction;

            var animator = Render.GetComponentInChildren<Animator>();
            animator.speed = 0.25f;

            Movement.SetBounds(bounds);
            Movement.StartRandomMovement();
        }
    }

    private void StartInteraction()
    {
        Movement.Stop();
        OnInteractionStarted?.Invoke();
    }

    public void Stop()
    {
        IsFollowing = false;
        Movement.StartRandomMovement();
    }

    public void Escort(Transform target)
    {
        IsFollowing = true;
        Movement.SetTarget(target);
    }

    public void Unescort()
    {
        IsFollowing = false;
        Movement.StartRandomMovement();
    }
}
