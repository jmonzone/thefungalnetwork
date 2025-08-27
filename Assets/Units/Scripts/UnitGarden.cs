using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class UnitGarden : MonoBehaviour, IJob
{
    [Header("References")]
    [SerializeField] private BuildReference buildReference;
    [SerializeField] private Item targetItem;

    [Header("Settings")]
    [SerializeField] private float positionOffset = 1f;

    [Header("Runtime")]
    [SerializeField] private bool isAble;
    [SerializeField] private bool isMoving = true;
    [SerializeField] private PlantSporeEmitter targetPlant;
    [SerializeField] private Vector3 targetPosition;

    private Coroutine debugCoroutine;

    bool IJob.IsAble => isAble;
    bool IJob.IsMoving => isMoving;
    Vector3 IJob.TargetPosition => targetPosition;

    public event UnityAction OnIsAbleChanged;
    public event UnityAction OnIsMovingChanged;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    private void OnEnable()
    {
        buildReference.OnBuildUpdated += BuildReference_OnBuildUpdated;
        BuildReference_OnBuildUpdated();
    }

    private void OnDisable()
    {
        buildReference.OnBuildUpdated -= BuildReference_OnBuildUpdated;
    }

    private void BuildReference_OnBuildUpdated()
    {
        var builds = buildReference.FindBuildControllersWhere(targetItem);
        if (builds.Count > 0)
        {
            var targetBuild = builds[0];
            foreach (var build in builds)
            {
                if (Vector3.Distance(transform.position, build.transform.position) < Vector3.Distance(transform.position, targetBuild.transform.position))
                {
                    targetBuild = build;
                }
            }

            targetPlant = targetBuild.GetComponent<PlantSporeEmitter>();
        }
        else targetPlant = null;

        isAble = targetPlant;
        OnIsAbleChanged?.Invoke();
        OnIsMovingChanged?.Invoke();
    }

    private void Update()
    {
        if (targetPlant == null)
        {
            StopDebugCoroutine();
            return;
        }

        // Face the targetBuild at all times
        FaceTarget();

        // Update destination only when needed
        FindNearestPosition();

        // Check movement state
        bool currentlyMoving = agent.hasPath &&
                               !(agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude < 0.01f);
        if (currentlyMoving != isMoving)
        {
            Debug.Log("isMoving changed");

            isMoving = currentlyMoving;
            OnIsMovingChanged?.Invoke();
        }

        // Start or stop the debug coroutine
        if (!isMoving && debugCoroutine == null)
        {
            Debug.Log("starting coroutine");
            debugCoroutine = StartCoroutine(EmitSporeRoutine());
        }
        else if (isMoving && debugCoroutine != null)
        {
            Debug.Log("stopping coroutine");
            StopDebugCoroutine();
        }
    }

    private void FaceTarget()
    {
        Vector3 lookDir = (targetPlant.transform.position - transform.position).normalized;
        lookDir.y = 0;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    private void FindNearestPosition()
    {
        // Direction from target to agent
        Vector3 dir = (transform.position - targetPlant.transform.position).normalized;
        if (dir == Vector3.zero)
        {
            dir = Random.insideUnitSphere.normalized;
            dir.y = 0;
        }

        // Offset position
        Vector3 desiredPos = targetPlant.transform.position + dir * positionOffset;

        // Ensure it's valid NavMesh position
        if (NavMesh.SamplePosition(desiredPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            // Only update destination if it's meaningfully different
            if ((targetPosition - hit.position).sqrMagnitude > 0.05f)
            {
                targetPosition = hit.position;
                agent.SetDestination(targetPosition);
            }
        }
    }

    private IEnumerator EmitSporeRoutine()
    {
        Debug.Log("Starting routine");
        //yield return new WaitForFixedUpdate();

        while (targetPlant != null && !isMoving)
        {
            // Only run if the agent is still next to the targetBuild
            float distanceToTarget = Vector3.Distance(transform.position, targetPlant.transform.position);
            if (distanceToTarget <= positionOffset + 0.1f)
            {
                targetPlant.EmitSpore();
                Debug.Log("Emit");
            }
            else
            {
                StopDebugCoroutine();
                yield break;
            }

            yield return new WaitForSeconds(4f);
        }
    }

    private void StopDebugCoroutine()
    {
        if (debugCoroutine != null)
        {
            StopCoroutine(debugCoroutine);
            debugCoroutine = null;
        }
    }
}
