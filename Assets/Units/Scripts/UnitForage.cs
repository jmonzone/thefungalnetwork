using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class UnitForage : MonoBehaviour, IJob
{
    [Header("References")]
    [SerializeField] private SporeReference sporeReference;
    [SerializeField] private BuildReference buildReference;
    [SerializeField] private Item targetItem;

    [Header("Settings")]
    [SerializeField] private float gardenOffset = 1f;
    [SerializeField] private float emitInterval = 4f;

    private NavMeshAgent agent;

    private SporeController targetSpore;
    private PlantSporeEmitter targetPlant;
    private Vector3 targetPosition;


    [Header("Runtime")]
    [SerializeField] private float emitTimer = 0f;
    [SerializeField] private bool isAble;
    [SerializeField] private bool isMoving;

    public bool IsAble => isAble;
    public bool IsMoving => isMoving;
    public Vector3 TargetPosition => targetSpore ? targetSpore.transform.position : targetPosition;

    public event UnityAction OnIsAbleChanged;
    public event UnityAction OnIsMovingChanged;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    private void OnEnable()
    {
        sporeReference.OnSporeControllersChanged += UpdateTargets;
        buildReference.OnBuildUpdated += UpdateTargets;
        UpdateTargets();
    }

    private void OnDisable()
    {
        sporeReference.OnSporeControllersChanged -= UpdateTargets;
        buildReference.OnBuildUpdated -= UpdateTargets;
    }

    private void UpdateTargets()
    {
        // Find closest spore
        targetSpore = null;
        foreach (var s in sporeReference.SporeControllers)
        {
            if (targetSpore == null || Vector3.Distance(transform.position, s.transform.position) < Vector3.Distance(transform.position, targetSpore.transform.position))
                targetSpore = s;
        }

        // Find closest build for gardening
        targetPlant = null;
        var builds = buildReference.FindBuildControllersWhere(targetItem);
        if (builds.Count > 0)
        {
            var closest = builds[0];
            foreach (var b in builds)
            {
                if (Vector3.Distance(transform.position, b.transform.position) < Vector3.Distance(transform.position, closest.transform.position))
                    closest = b;
            }
            targetPlant = closest.GetComponent<PlantSporeEmitter>();
        }

        bool prevAble = isAble;
        isAble = targetSpore != null || targetPlant != null;
        if (prevAble != isAble)
            OnIsAbleChanged?.Invoke();
    }

    private void Update()
    {
        if (!isAble)
        {
            StopMovement();
            emitTimer = 0f;
            return;
        }

        // Forage takes priority
        if (targetSpore != null)
        {
            emitTimer = 0f; // stop gardening
            HandleForage();
        }
        else if (targetPlant != null)
        {
            HandleGarden();
        }
    }

    private void HandleForage()
    {
        float distance = Vector3.Distance(transform.position, targetSpore.transform.position);
        bool prevMoving = isMoving;
        isMoving = distance > 0.5f;
        OnIsMovingChangedIfNeeded(prevMoving);

        if (isMoving)
        {
            agent.SetDestination(targetSpore.transform.position);
            FaceTarget(targetSpore.transform.position);
        }
        else
        {
            agent.ResetPath();
            targetSpore.Collect();
        }
    }

    private void HandleGarden()
    {
        FaceTarget(targetPlant.transform.position);
        MoveToGardenPosition();

        bool prevMoving = isMoving;
        isMoving = agent.hasPath && !(agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude < 0.01f);
        OnIsMovingChangedIfNeeded(prevMoving);

        // Handle timed spore emission
        if (!isMoving)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPlant.transform.position);
            if (distanceToTarget <= gardenOffset + 0.1f)
            {
                emitTimer += Time.deltaTime;
                if (emitTimer >= emitInterval)
                {
                    targetPlant.EmitSpore();
                    emitTimer = 0f;
                }
            }
            else
            {
                emitTimer = 0f;
            }
        }
        else
        {
            emitTimer = 0f;
        }
    }

    private void MoveToGardenPosition()
    {
        Vector3 dir = (transform.position - targetPlant.transform.position).normalized;
        if (dir == Vector3.zero)
        {
            dir = Random.insideUnitSphere.normalized;
            dir.y = 0;
        }

        Vector3 desiredPos = targetPlant.transform.position + dir * gardenOffset;
        if (NavMesh.SamplePosition(desiredPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            if ((targetPosition - hit.position).sqrMagnitude > 0.05f)
            {
                targetPosition = hit.position;
                agent.SetDestination(targetPosition);
            }
        }
    }

    private void FaceTarget(Vector3 target)
    {
        Vector3 lookDir = (target - transform.position).normalized;
        lookDir.y = 0;
        if (lookDir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);
    }

    private void StopMovement()
    {
        bool prevMoving = isMoving;
        isMoving = false;
        OnIsMovingChangedIfNeeded(prevMoving);
        agent.ResetPath();
    }

    private void OnIsMovingChangedIfNeeded(bool prevMoving)
    {
        if (prevMoving != isMoving)
            OnIsMovingChanged?.Invoke();
    }
}
