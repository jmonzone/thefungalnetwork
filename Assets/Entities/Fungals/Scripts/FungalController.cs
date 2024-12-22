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
    [SerializeField] private Controller controller;

    public FungalModel Model { get; private set; }
    public MovementController Movement { get; private set; }

    public event UnityAction OnInitialized;

    private void Awake()
    {
        Movement = GetComponent<MovementController>();

        var mountController = GetComponent<MountController>();
        mountController.OnMounted += () =>
        {
            controller.SetMovement(mountController.Mount.Movement);
        };

        mountController.OnUnmounted += () =>
        {
            controller.SetMovement(Movement);
        };
    }

    //todo: centralize between GroveManager and Launcher
    public void Initialize(FungalModel model, bool isGrove = true)
    {
        Debug.Log($"initializing fungal controller {model}");

        Model = model;

        if (model)
        {
            name = $"Fungal Controller - {model.name}";
            var render = Instantiate(model.Data.Prefab, transform);
            render.transform.localScale = Vector3.one;

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
