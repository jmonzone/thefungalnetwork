using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public interface IAbilityHolder
{
    public bool CanBePickedUp(FungalController fungal);
    public Vector3 Position { get; }
}

public class FungalAI : MonoBehaviour
{
    [SerializeField] private GameReference game;

    [SerializeField] private bool autoStartAI = false;

    private NavMeshAgent agent;
    private FungalController fungal;
    private Ability innateFungalAbility;
    private Coroutine fungalStateCoroutine;

    private Vector3 targetPosition;



    [SerializeField] private float minAbilityDelay = 1f;
    [SerializeField] private float maxAbilityDelay = 3f;

    private float lastAbilityTime = 0f;
    private float nextAbilityDelay = 0f;

    private Vector3 cachedTargetPosition = Vector3.positiveInfinity;

    [Header("Debug")]
    private IAbilityHolder targetAbilityHolder;
    private List<IAbilityHolder> allAbilityHolders = new List<IAbilityHolder>();

    [SerializeField] private FungalController targetFungal;
    [SerializeField] private List<FungalController> allFungals = new List<FungalController>();

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        fungal = GetComponent<FungalController>();

        allAbilityHolders = FindObjectsOfType<MonoBehaviour>()
            .OfType<IAbilityHolder>()
            .ToList();

        fungal.OnAbilityChanged += () =>
        {
            lastAbilityTime = Time.time;
            nextAbilityDelay = Random.Range(minAbilityDelay, maxAbilityDelay);
        };

        fungal.OnInitialized += FungalController_OnInitialized;
    }

    private IEnumerator Start()
    {
        if (autoStartAI)
        {
            yield return new WaitForSeconds(2f);
            StartAI();
        }
    }

    private void Update()
    {
        agent.speed = fungal.Movement.CalculatedSpeed;
    }

    private void FungalController_OnInitialized()
    {
        var abilityTemplate = fungal.Data.Ability;
        innateFungalAbility = Instantiate(abilityTemplate);
        innateFungalAbility.Initialize(fungal);
    }

    public void StartAI()
    {
        allFungals = FindObjectsOfType<FungalController>().ToList();

        //if isOwner && isAi
        agent.enabled = true;

        // Ensure AI starts on the NavMesh
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position); // Teleport AI to valid NavMesh point
        }
        else
        {
            Debug.LogError("AI is outside the NavMesh!");
        }

        agent.isStopped = false; // Make sure it's not paused

        // Start the coroutine when the object is enabled
        fungalStateCoroutine = StartCoroutine(FungalStateMachine());

        fungal.OnDeath += Fungal_OnDeath;
    }

    private void Fungal_OnDeath(bool arg0)
    {
        StopAI();

        fungal.OnDeath -= Fungal_OnDeath;
        fungal.OnRespawnComplete += Fungal_OnRespawnComplete;
    }

    private void Fungal_OnRespawnComplete()
    {
        StartAI();

        fungal.OnRespawnComplete -= Fungal_OnRespawnComplete;
    }

    public void StopAI()
    {
        agent.enabled = false;

        // Stop the coroutine when the object is disabled
        if (fungalStateCoroutine != null)
        {
            StopCoroutine(fungalStateCoroutine);
            fungalStateCoroutine = null;
        }
    }

    private float AbilityRange
    {
        get
        {
            if (fungal.Ability is DirectionalAbility directionalAbility)
            {
                return directionalAbility.Range;
            }
            else return 3f;
        }
    }

    private bool HasClearPath
    {
        get
        {
            if (!fungal.Ability) return false;
            if (!targetFungal) return false;

            if (Vector3.Distance(transform.position, targetFungal.transform.position) > AbilityRange) return false;

            bool useTrajectory = fungal.Ability is DirectionalAbility da && da.UseTrajectory;
            return useTrajectory || !agent.Raycast(targetFungal.transform.position, out _);
        }
    }

    private bool CanUsePowerUpAbility
    {
        get
        {
            if (!fungal.Ability) return false;
            if (!targetFungal) return false;
            if (!HasClearPath) return false;
            if (IsWaitingToUseAbility) return false;
            return true;
        }
    }

    private bool IsWaitingToUseAbility
    {
        get
        {
            return Time.time < lastAbilityTime + nextAbilityDelay;
        }
    }

    private IEnumerator FungalStateMachine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            if (!(innateFungalAbility is DirectionalAbility) && !innateFungalAbility.IsOnCooldown)
            {
                innateFungalAbility.CastAbility();
            }

            targetFungal = allFungals
                   .Where(f => f != fungal && !f.IsDead)
                   .OrderBy(f => Vector3.Distance(transform.position, f.transform.position))
                   .FirstOrDefault();

            if (!fungal.Ability)
            {
                targetAbilityHolder = allAbilityHolders
                .Where(target => target.CanBePickedUp(fungal) && IsOnNavMesh(target.Position))
                .OrderBy(target => Vector3.Distance(transform.position, target.Position))
                .FirstOrDefault();

                if (targetAbilityHolder != null) targetPosition = targetAbilityHolder.Position;
            }
            else if (!targetFungal)
            {

            }
            else if (!HasClearPath || IsWaitingToUseAbility)
            {
                float distanceToCached = Vector3.Distance(transform.position, cachedTargetPosition);
                float distanceFromCachedToTarget = Vector3.Distance(cachedTargetPosition, targetFungal.transform.position);

                float avoidDistance = AbilityRange * 0.75f;
                bool isCachedInRange = distanceFromCachedToTarget <= AbilityRange && distanceFromCachedToTarget >= avoidDistance;
                bool hasReachedCached = distanceToCached < 0.5f;

                if (!isCachedInRange || hasReachedCached)
                {
                    // Get the direction from AI to target
                    Vector3 toAI = (transform.position - targetFungal.transform.position).normalized;

                    // Randomize angle around this direction
                    float baseAngle = Mathf.Atan2(toAI.z, toAI.x);
                    float angleOffset = Random.Range(-45f, 45f) * Mathf.Deg2Rad;
                    float finalAngle = baseAngle + angleOffset;

                    // Calculate the new target position with offset
                    float radius = Random.Range(avoidDistance, AbilityRange);
                    cachedTargetPosition = targetFungal.transform.position + new Vector3(Mathf.Cos(finalAngle), 0, Mathf.Sin(finalAngle)) * radius;
                }

                targetPosition = cachedTargetPosition;

            }
            else if (CanUsePowerUpAbility)
            {
                if (fungal.Ability is DirectionalAbility directional)
                {
                    directional.CastAbility(targetFungal.transform.position);
                    lastAbilityTime = Time.time;
                    nextAbilityDelay = directional.Cooldown.Cooldown + Random.Range(minAbilityDelay, maxAbilityDelay);
                }
            }

            yield return UseMoveAction(targetPosition);
        }
    }

    private IEnumerator UseMoveAction(Vector3 targetPosition)
    {
        if (innateFungalAbility is IMovementAbility movementAbility && innateFungalAbility is DirectionalAbility directionalAbility && !directionalAbility.IsOnCooldown)
        {
            if (Vector3.Distance(transform.position, targetPosition) > directionalAbility.Range)
            {
                agent.enabled = false;

                Vector3 dashTargetPosition = GetDashTargetPosition(targetPosition, movementAbility);
                directionalAbility.CastAbility(dashTargetPosition);

                yield return new WaitWhile(() => directionalAbility.InUse);

                agent.enabled = true;
            }
        }

        agent.SetDestination(targetPosition);
    }

    private Vector3 GetDashTargetPosition(Vector3 targetPosition, IMovementAbility movementAbility)
    {
        Vector3 origin = transform.position;
        Vector3 direction = targetPosition - origin;

        float distanceToTarget = direction.magnitude;

        Vector3 intendedPosition = origin + direction.normalized * movementAbility.Range;


        // If the ability ignores obstacles, just use the direct target or capped range
        if (movementAbility.IgnoreObstacles)
        {
            intendedPosition = distanceToTarget <= movementAbility.Range
                ? targetPosition
                : origin + direction.normalized * movementAbility.Range;

            return intendedPosition;
        }


        // If path is clear, use it
        if (!NavMesh.Raycast(origin, intendedPosition, out _, NavMesh.AllAreas) &&
            NavMesh.SamplePosition(intendedPosition, out NavMeshHit directHit, 1f, NavMesh.AllAreas))
        {
            return directHit.position;
        }

        // Else, check nearby directions
        Vector3 bestPosition = origin;
        float closestDistance = float.MaxValue;

        for (int i = -4; i <= 4; i++)
        {
            float angle = i * 22.5f; // Spread across ±90°
            Vector3 testDir = Quaternion.Euler(0, angle, 0) * direction.normalized;
            Vector3 testPos = origin + testDir * movementAbility.Range;

            if (!NavMesh.Raycast(origin, testPos, out _, NavMesh.AllAreas) &&
                NavMesh.SamplePosition(testPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                float distance = Vector3.Distance(hit.position, targetPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestPosition = hit.position;
                }
            }
        }

        return bestPosition;
    }

    // Helper function to check if the position is valid on the NavMesh
    private bool IsOnNavMesh(Vector3 position)
    {
        return NavMesh.SamplePosition(position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas);
    }
}
