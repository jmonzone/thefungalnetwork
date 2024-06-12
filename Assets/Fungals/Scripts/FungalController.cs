using UnityEngine;
using UnityEngine.Events;

public abstract class EntityController : MonoBehaviour
{
    [SerializeField] private bool hasInteraction = false;

    public bool HasInteraction => hasInteraction;
    public abstract Sprite ActionImage { get; }
    public abstract Color ActionColor { get; }
    public abstract string ActionText { get; }

    public abstract void UseAction();
}

public enum FungalState
{
    IDLE,
    RANDOM,
    ESCORT,
    TARGET,
}

public class FungalController : EntityController
{
    [Header("References")]
    [SerializeField] private Transform indicatorAnchor;
    [SerializeField] private RectTransform hungerIndicator;
    [SerializeField] private Camera spotlightCamera;

    public FungalModel Model { get; private set; }
    public GameObject Render { get; private set; }
    public bool IsFollowing { get; set; }

    public Camera SpotlightCamera => spotlightCamera;

    public override Sprite ActionImage => Model.Data.ActionImage;
    public override Color ActionColor => Model.Data.ActionColor;
    public override string ActionText => "Talk";

    private Camera mainCamera;
    private MoveController movement;
    private Animator animator;
    private float hungerTimer;
    private float idleTimer;
    private FungalState state;

    public event UnityAction OnTalkStart;

    private void Awake()
    {
        mainCamera = Camera.main;
        movement = GetComponent<MoveController>();
    }

    public void SetFungal(FungalModel model)
    {
        Debug.Log($"initializing fungal controller {model}");

        Model = model;

        if (model)
        {
            name = $"Fungal Controller - {model.name}";
            Render = Instantiate(model.Data.Prefab, transform);
            Render.transform.localScale = Vector3.one;

            animator = Render.GetComponentInChildren<Animator>();
            animator.speed = 0.25f;

            movement.SetSpeed(1f + model.Speed * 0.1f);

            SetState(FungalState.IDLE);
        }
    }

    public void MoveToPosition(Vector3 position, Transform lookTarget = null)
    {
        movement.SetPosition(position, () =>
        {
            if (lookTarget) movement.SetLookTarget(lookTarget);
        });
    }

    public void MoveToTarget(Transform target)
    {
        movement.SetTarget(target);
        SetState(FungalState.TARGET);
    }

    public void Stop()
    {
        IsFollowing = false;
        movement.Stop();
        SetState(FungalState.IDLE);
    }

    public void Escort(Transform target)
    {
        IsFollowing = true;
        movement.SetTarget(target);
        SetState(FungalState.ESCORT);
    }

    public void Unescort()
    {
        IsFollowing = false;
        Debug.Log("isfollowing" + IsFollowing);
        movement.Stop();
        SetState(FungalState.IDLE);
    }

    private void Update()
    {
        UpdateHunger();

        switch (state)
        {
            case FungalState.IDLE:
                idleTimer += Time.deltaTime;
                if (idleTimer > 5f) SetState(FungalState.RANDOM);
                break;
            case FungalState.RANDOM:
                if (!movement.IsMovingToPosition) SetState(FungalState.IDLE);
                break;
        }

    }

    private void SetState(FungalState state)
    {
        this.state = state;

        switch (state)
        {
            case FungalState.IDLE:
                idleTimer = Random.Range(0f, 3f);
                animator.SetBool("isMoving", false);
                break;

            case FungalState.RANDOM:
                animator.SetBool("isMoving", true);
                MoveToPosition(Utility.RandomXZVector * 5f);
                break;
        }
    }

    public override void UseAction()
    {
        OnTalkStart?.Invoke();
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
