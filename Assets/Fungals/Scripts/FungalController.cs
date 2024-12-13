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
    [SerializeField] private ProximityAction proximityAction;
    [SerializeField] private MovementController movement;
    [SerializeField] private HealthSlider healthSlider;
    [SerializeField] private Image interactionOutline;
    [SerializeField] private Image interactionBackground;
    [SerializeField] private Image interactionImage;

    public FungalModel Model { get; private set; }
    public GameObject Render { get; private set; }
    public MovementController Movement => movement;

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

            movement.OnJump += () => animator.Play("Jump");
            movement.SetMaxJumpCount(model.Data.Type == FungalType.SKY ? 2 : 1);
            this.movement.StartRandomMovement();

            var movementAnimations = GetComponentInChildren<MovementAnimations>();
            movementAnimations.Initalize();

            //interactionOutline.color = model.Data.EggColor;
            //interactionBackground.color = model.Data.ActionColor;
            //interactionImage.sprite = model.Data.ActionImage;
        }
    }
}
