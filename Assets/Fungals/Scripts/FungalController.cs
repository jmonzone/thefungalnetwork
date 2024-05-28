using System.Collections.Generic;
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    [SerializeField] private bool hasInteraction = false;

    public bool HasInteraction => hasInteraction;
    public abstract Sprite ActionImage { get; }
    public abstract Color Color { get; }
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
    [SerializeField] private float distanceThreshold = 1f;

    [Header("References")]
    [SerializeField] private GameObject placeholder;
    [SerializeField] private Transform indicatorAnchor;
    [SerializeField] private RectTransform hungerIndicator;

    [Header("Debug")]
    [SerializeField] private Vector3 origin;
    [SerializeField] private float timer;
    [SerializeField] private float hungerTimer;
    [SerializeField] private Transform target;
    [SerializeField] private FishController targetFish;
    [SerializeField] private List<FishController> fish = new List<FishController>();

    public FungalInstance FungalInstance { get; private set; }
    public bool IsFollowing { get; set; } = false;

    public override Sprite ActionImage => FungalInstance.Data.ActionImage;
    public override Color Color => FungalInstance.Data.Color;

    private Camera mainCamera;

    private void Awake()
    {
        Destroy(placeholder);
        placeholder = null;

        origin = transform.position;
        mainCamera = Camera.main;
    }

    public void Initialize(FungalInstance fungalInstance)
    {
        Debug.Log($"initializing fungal controller {fungalInstance}");
        FungalInstance = fungalInstance;

        if (fungalInstance)
        {
            var petObject = Instantiate(fungalInstance.Data.Prefab, transform);
            petObject.transform.localScale = Vector3.one;

            var animator = petObject.GetComponentInChildren<Animator>();
            animator.speed = 0.25f;
        }
    }

    public void Escort(Transform target)
    {
        IsFollowing = true;
        SetTarget(target);
    }

    public void Unescort()
    {
        IsFollowing = false;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        
        randomizePositions = !target;
    }

    public void SetFish(List<FishController> fish)
    {
        this.fish = fish;
    }

    private Vector3 targetPosition = Vector3.right * 3f;
    private Vector3 TargetPosition
    {
        get
        {
            if (target)
            {
                return target.position;
            }
            else if (randomizePositions)
            {
                if (Vector3.Distance(transform.position, targetPosition) < distanceThreshold)
                {
                    var random = (Vector3) Random.insideUnitCircle.normalized * 5f;
                    random.z = random.y;
                    random.y = 0;
                    targetPosition = random;
                }

                return targetPosition;
            }
            else
            {
                var x = Mathf.Cos(timer);
                var z = Mathf.Sin(timer);
                return origin + offset - new Vector3(x, 0, z) * idleRadius;
            }
        }
    }

    private void Update()
    {
        if (FungalInstance)
        {
            hungerTimer += Time.deltaTime;

            if (hungerTimer > 5)
            {
                FungalInstance.Hunger -= 1;
                hungerTimer = 0;
            }

            if (FungalInstance.Hunger < 30)
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

        var direction = TargetPosition - transform.position;
        transform.forward = Vector3.Lerp(transform.forward, direction, rotationSpeed * Time.deltaTime);

        if (direction.magnitude > distanceThreshold)
        {
            var fixedSpeed = speed;
            if (target) fixedSpeed *= 2;
            transform.position += fixedSpeed * Time.deltaTime * direction.normalized;
        }
        else if (targetFish)
        {
            targetFish.Catch();
            target = null;
            targetFish = null;
            timer = 0;
        }

        if (timer > autoFishCooldown)
        {
            targetFish = null;

            var closestDistance = Mathf.Infinity;

            foreach (var _fish in fish)
            {
                if (_fish.IsAttacted || _fish.IsCaught) continue;
                var distance = Vector3.Distance(_fish.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = _fish.transform;
                    targetFish = _fish;
                }
            }
        }

        timer += Time.deltaTime;
    }
}
