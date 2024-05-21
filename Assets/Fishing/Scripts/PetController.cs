using System.Collections.Generic;
using UnityEngine;

public class PetController : MonoBehaviour
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

    public PetInstance PetInstance { get; private set; }
    private Vector3 origin;
    private float timer;
    private Transform target;
    private FishController targetFish;
    private List<FishController> fish = new List<FishController>();

    private Camera mainCamera;

    private void Awake()
    {
        Destroy(placeholder);
        placeholder = null;

        origin = transform.position;
        mainCamera = Camera.main;
    }

    public void SetPet(PetInstance pet)
    {
        PetInstance = pet;

        if (pet)
        {
            var petObject = Instantiate(pet.Data.Prefab, transform);
            petObject.transform.localScale = Vector3.one;

            var animator = petObject.GetComponentInChildren<Animator>();
            animator.speed = 0.25f;

            if (pet.Data.Type == PetType.SKY)
            {
                origin.y = 5;
                transform.position = origin;
            }
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        randomizePositions = false;
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
            if (randomizePositions)
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
            if (target)
            {
                return target.position;
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
        if (PetInstance)
        {
            PetInstance.Hunger -= Time.deltaTime;

            if (PetInstance.Hunger < 30)
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
        if (direction.magnitude > distanceThreshold)
        {
            transform.position += speed * Time.deltaTime * direction.normalized;
            transform.forward = Vector3.Lerp(transform.forward, direction, rotationSpeed * Time.deltaTime);
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
