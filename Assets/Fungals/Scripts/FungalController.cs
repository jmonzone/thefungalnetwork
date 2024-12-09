using UnityEngine;

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
    [SerializeField] private Controllable controllable;

    public FungalModel Model { get; private set; }
    public GameObject Render { get; private set; }
    public Controllable Controllable => controllable;

    public void Initialize(FungalModel model)
    {
        Debug.Log($"initializing fungal controller {model}");

        Model = model;

        if (model)
        {
            name = $"Fungal Controller - {model.name}";
            Render = Instantiate(model.Data.Prefab, transform);
            Render.transform.localScale = Vector3.one;

            var animator = Render.GetComponentInChildren<Animator>();
            animator.speed = 0.25f;

            controllable.Movement.StartRandomMovement();
        }
    }
}
