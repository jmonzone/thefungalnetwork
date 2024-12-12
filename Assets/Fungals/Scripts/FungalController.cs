using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Controller controller;
    [SerializeField] private Controllable controllable;
    [SerializeField] private HealthSlider healthSlider;

    public FungalModel Model { get; private set; }
    public GameObject Render { get; private set; }
    public Controllable Controllable => controllable;

    private void OnEnable()
    {
        //controller.OnUpdate += () => healthSlider.gameObject.SetActive(controllable == controller.Controllable);
    }

    private void OnDisable()
    {
        //controller.OnUpdate -= () => healthSlider.gameObject.SetActive(controllable == controller.Controllable);
    }

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

            var movement = GetComponentInChildren<MovementController>();
            movement.OnJump += () => animator.Play("Jump");
            movement.SetMaxJumpCount(model.Data.Type == FungalType.SKY ? 2 : 1);
            controllable.Movement.StartRandomMovement();

            var movementAnimations = GetComponentInChildren<MovementAnimations>();
            movementAnimations.Initalize();
        }
    }
}
