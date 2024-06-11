using System.Collections.Generic;
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    [SerializeField] private bool hasInteraction = false;

    public bool HasInteraction => hasInteraction;
    public abstract Sprite ActionImage { get; }
    public abstract Color ActionColor { get; }
    public abstract string ActionText { get; }

    public abstract void UseAction();
}

public class FungalController : EntityController
{
    [Header("Configuration")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float idleRadius = 4f;
    [SerializeField] private float autoFishCooldown = 4f;
    [SerializeField] private float flightHeight = 5f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool randomizePositions = false;

    private float targetDistanceThreshold = 0.1f;
    private float followDistanceThreshold = 2.5f;

    [Header("References")]
    [SerializeField] private Transform indicatorAnchor;
    [SerializeField] private RectTransform hungerIndicator;
    [SerializeField] private Camera spotlightCamera;

    [Header("Debug")]
    [SerializeField] private Vector3 origin;
    [SerializeField] private float timer;
    [SerializeField] private float hungerTimer;
    [SerializeField] private Transform target;
    [SerializeField] private FishController targetFish;
    [SerializeField] private List<FishController> fish = new List<FishController>();

    public FungalInstance Model { get; private set; }
    public bool IsFollowing { get; set; } = false;

    public override Sprite ActionImage => Model.Data.ActionImage;
    public override Color ActionColor => Model.Data.ActionColor;
    public override string ActionText => "Talk";

    public Camera SpotlightCamera => spotlightCamera;
    public GameObject Model3D => model3D;

    private Camera mainCamera;
    private GameObject model3D;

    private MoveController movement;

    private void Awake()
    {
        origin = transform.position;
        mainCamera = Camera.main;
        movement = GetComponent<MoveController>();
    }

    public void SetFungal(FungalInstance fungalInstance)
    {
        Debug.Log($"initializing fungal controller {fungalInstance}");
        Model = fungalInstance;

        if (fungalInstance)
        {
            name = $"Fungal Controller - {fungalInstance.name}";
            model3D = Instantiate(fungalInstance.Data.Prefab, transform);
            model3D.transform.localScale = Vector3.one;

            var animator = model3D.GetComponentInChildren<Animator>();
            animator.speed = 0.25f;

            speed = 0.5f + fungalInstance.Speed * 0.1f;
        }
    }

    public void MoveToPosition(Vector3 position, Transform lookTarget = null)
    {
        movement.SetPosition(position, () =>
        {
            Debug.Log("complete");
            if (lookTarget) movement.SetLookTarget(lookTarget);
        });
    }

    public void MoveToTarget(Transform target)
    {
        movement.SetTarget(target);
    }

    public void Stop()
    {
        IsFollowing = false;
        movement.Stop();
    }

    public void Escort(Transform target)
    {
        IsFollowing = true;
        movement.SetTarget(target);
    }

    public void Unescort()
    {
        IsFollowing = false;
        movement.Stop();
    }

    public void SetFish(List<FishController> fish)
    {
        this.fish = fish;
    }

    private void Update()
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

        //var direction = TargetPosition - transform.position;
        //transform.forward = Vector3.Lerp(transform.forward, direction, rotationSpeed * Time.deltaTime);

        //if (direction.magnitude > distanceThreshold)
        //{
        //    var fixedSpeed = speed;
        //    if (target) fixedSpeed *= 2;
        //    transform.position += fixedSpeed * Time.deltaTime * direction.normalized;
        //}
        //else if (targetFish)
        //{
        //    targetFish.Catch();
        //    target = null;
        //    targetFish = null;
        //    timer = 0;
        //}

        //if (timer > autoFishCooldown)
        //{
        //    targetFish = null;

        //    var closestDistance = Mathf.Infinity;

        //    foreach (var _fish in fish)
        //    {
        //        if (_fish.IsAttacted || _fish.IsCaught) continue;
        //        var distance = Vector3.Distance(_fish.transform.position, transform.position);
        //        if (distance < closestDistance)
        //        {
        //            closestDistance = distance;
        //            target = _fish.transform;
        //            targetFish = _fish;
        //        }
        //    }
        //}

        //timer += Time.deltaTime;
    }

    public override void UseAction()
    {
    }
}
