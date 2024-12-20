using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField] private HealthSlider healthSlider;
    [SerializeField] private Image interactionOutline;
    [SerializeField] private Image interactionBackground;
    [SerializeField] private Image interactionImage;

    private ProximityAction proximityAction;

    public FungalModel Model { get; private set; }
    public GameObject Render { get; private set; }
    public MovementController Movement { get; private set; }

    public event UnityAction OnInitialized;

    private void Awake()
    {
        proximityAction = GetComponent<ProximityAction>();
        proximityAction.OnUse += () =>
        {
            controller.SetMovement(Movement);
        };

        Movement = GetComponent<MovementController>();
    }

    private void OnEnable()
    {
        controller.OnIsPossessingChanged += Controller_OnIsPossessingChanged;
    }

    private void Controller_OnIsPossessingChanged()
    {
        proximityAction.SetInteractable(!controller.IsPossessing && !controller.Fungal);
    }

    private void OnDisable()
    {
        controller.OnIsPossessingChanged -= Controller_OnIsPossessingChanged;
    }

    //todo: centralize between GroveManager and Launcher
    public void Initialize(FungalModel model, bool isGrove = true)
    {
        Debug.Log($"initializing fungal controller {model}");

        Model = model;

        if (model)
        {
            name = $"Fungal Controller - {model.name}";
            Render = Instantiate(model.Data.Prefab, transform);
            Render.transform.localScale = Vector3.one;

            if (isGrove)
            {
                Movement.SetMaxJumpCount(model.Data.Type == FungalType.SKY ? 2 : 1);
                Movement.StartRandomMovement();
            }

            InitializeAnimations();
        }
    }

    public void InitializeAnimations()
    {
        var animator = GetComponentInChildren<Animator>();
        animator.speed = 0.25f;

        Movement.OnJump += () => animator.Play("Jump");

        var movementAnimations = GetComponentInChildren<MovementAnimations>();
        movementAnimations.Initalize();
    }
}
