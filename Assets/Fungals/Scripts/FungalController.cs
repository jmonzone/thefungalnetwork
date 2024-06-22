using UnityEngine;
using UnityEngine.Events;

public enum FungalState
{
    RANDOM,
    ESCORT,
    TARGET,
}

[RequireComponent(typeof(MoveController))]
[RequireComponent(typeof(ProximityAction))]
public class FungalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform indicatorAnchor;
    [SerializeField] private RectTransform hungerIndicator;
    [SerializeField] private Camera spotlightCamera;

    public FungalModel Model { get; private set; }
    public GameObject Render { get; private set; }
    public MoveController Movement { get; private set; }
    public bool IsFollowing { get; set; }

    public Camera SpotlightCamera => spotlightCamera;

    private Camera mainCamera;
    private float hungerTimer;

    public event UnityAction OnTalkStart;

    private void Awake()
    {
        mainCamera = Camera.main;
        Movement = GetComponent<MoveController>();
        hungerIndicator.gameObject.SetActive(false);
    }

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
            proximityAction.Sprite = Model.Data.ActionImage;
            proximityAction.Color = Model.Data.ActionColor;
            proximityAction.OnUse += () => OnTalkStart?.Invoke();

            var animator = Render.GetComponentInChildren<Animator>();
            animator.speed = 0.25f;

            Movement.SetAnimator(animator);
            Movement.SetBounds(bounds);
            Movement.SetSpeed(1f + model.Speed * 0.1f);
            Movement.StartRandomMovement();
        }
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

    private void Update()
    {
        //UpdateHunger();
    }

    private void UpdateHunger()
    {
        if (Model)
        {
            hungerTimer += Time.deltaTime;

            if (hungerTimer > 5)
            {
                Model.Hunger -= 5 / (1 + Model.Stamina * 0.1f);
                hungerTimer = 0;
            }

            if (Model.Hunger < 30)
            {
                hungerIndicator.gameObject.SetActive(true);
                var position = mainCamera.WorldToScreenPoint(indicatorAnchor.transform.position);
                hungerIndicator.position = position;
            }
            else
            {
                hungerIndicator.gameObject.SetActive(false);
            }
        }
    }
}
