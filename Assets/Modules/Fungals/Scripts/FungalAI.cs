using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FungalAI : MonoBehaviour
{
    private enum FungalState
    {
        FIND_FISH,
        THROW_FISH,
    }

    [SerializeField] private GameReference game;
    [SerializeField] private FungalState currentState;

    [SerializeField] private float minDashInterval = 2f; // Minimum time between dashes
    [SerializeField] private float maxDashInterval = 5f; // Maximum time between dashes

    private float lastDashTime = 0f;  // Stores the last dash time
    private float nextDashTime = 0f; // Next randomized dash time

    private NavMeshAgent agent;
    private Movement movement;
    private FungalController fungal;
    private Ability ability;


    private FishPickup fishPickup;
    private Coroutine fungalStateCoroutine;

    [Header("Debug")]
    [SerializeField] private FishController targetFish;
    [SerializeField] private List<FishController> allFish = new List<FishController>();

    [SerializeField] private FungalController targetFungal;
    [SerializeField] private List<FungalController> allFungals = new List<FungalController>();


    private void Awake()
    {
        movement = GetComponent<Movement>();
        agent = GetComponent<NavMeshAgent>();
        //agent.isStopped = true;
        fungal = GetComponent<FungalController>();

        allFish = FindObjectsOfType<FishController>().ToList();

        fishPickup = GetComponent<FishPickup>();

        var fungalController = GetComponent<FungalController>();
        fungalController.OnInitialized += FungalController_OnInitialized;
        fungalController.OnDeath += _ => StopAI();
        fungalController.OnRespawnComplete += () => StartAI();
    }

    private void FungalController_OnInitialized()
    {
        var abilityTemplate = fungal.Data.Ability;
        ability = Instantiate(abilityTemplate);
        ability.Initialize(fungal);
    }

    public void StartAI()
    {
        allFungals = FindObjectsOfType<FungalController>().ToList();

        //if isOwwner && is ai
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

        SetState(FungalState.FIND_FISH); // Start moving

        // Start the coroutine when the object is enabled
        fungalStateCoroutine = StartCoroutine(FungalStateMachine());
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


    private IEnumerator FungalStateMachine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            if (!(ability is DirectionalAbility) && !ability.IsOnCooldown)
            {
                ability.CastAbility();
            }

            switch (currentState)
            {
                case FungalState.FIND_FISH:
                    yield return FindAndPickUpFish();
                    break;
                case FungalState.THROW_FISH:
                    yield return ThrowFish();
                    break;
            }

            yield return null;
        }
    }

    // Helper function to check if the position is valid on the NavMesh
    private bool IsOnNavMesh(Vector3 position)
    {
        return NavMesh.SamplePosition(position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas);
    }

    private IEnumerator FindAndPickUpFish()
    {
        targetFish = allFish
                 .Where(fish => !fish.IsPickedUp && IsOnNavMesh(fish.transform.position))
                 .OrderBy(fish => Vector3.Distance(transform.position, fish.transform.position))
                 .FirstOrDefault();

        if (targetFish != null)
        {
            yield return UseMoveAction(targetFish.transform.position);
        }

        if (fishPickup.Fish)
        {
            SetState(FungalState.THROW_FISH);
        }

        yield return null;
    }

    private Vector3 GetDashTargetPosition(Vector3 targetPosition, IMovementAbility movementAbility )
    {
        Vector3 origin = transform.position;
        Vector3 direction = targetPosition - origin;

        Vector3 intendedPosition = direction.magnitude <= movementAbility.Range
            ? targetPosition
            : origin + direction.normalized * movementAbility.Range;

        // If the ability ignores obstacles, just go straight and sample NavMesh
        if (movementAbility.IgnoreObstacles)
        {
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



    private IEnumerator UseMoveAction(Vector3 targetPosition)
    {
        yield return null;

        if (ability is IMovementAbility movementAbility && ability is DirectionalAbility directionalAbility)
        {

            float cooldownTime = ability.Cooldown.RemainingTime; // Get cooldown time


            // Check if the dash is ready (not on cooldown) and enough time has passed
            if (!ability.IsOnCooldown && Time.time >= lastDashTime + nextDashTime + cooldownTime)
            {
                Debug.Log("Fungal AI Casting ability");

                agent.enabled = false;

                Vector3 dashTargetPosition = GetDashTargetPosition(targetPosition, movementAbility);
                directionalAbility.CastAbility(dashTargetPosition);

                lastDashTime = Time.time;  // Update last dash time

                // Randomize the next dash interval within the defined range
                nextDashTime = Random.Range(minDashInterval, maxDashInterval);

                yield return new WaitWhile(() => directionalAbility.InUse);

                agent.enabled = true;
            }
        }

        agent.SetDestination(targetPosition);
    }

    private float timeInSlingRange = 0f;
    private float requiredTimeInRange = 0.5f;
    private float comfortableRange;

    private IEnumerator ThrowFish()
    {
        targetFungal = allFungals
            .Where(f => f != fungal && !f.IsDead)
            .OrderBy(f => Vector3.Distance(transform.position, f.transform.position))
            .FirstOrDefault();

        if (!fishPickup.Fish)
        {
            SetState(FungalState.FIND_FISH);
            yield break;
        }

        if (!targetFungal)
            yield break;

        comfortableRange = fishPickup.Fish.ThrowFish.Range * 0.75f;

        Vector3 predictedTarget = targetFungal.transform.position + targetFungal.Movement.SpeedDelta * targetFungal.transform.forward;
        float distance = Vector3.Distance(transform.position, predictedTarget);

        timeInSlingRange += Time.deltaTime;

        // Calculate initial angle based on the direction to the target
        Vector3 directionToTarget = predictedTarget - transform.position;
        float initialAngle = Mathf.Atan2(directionToTarget.z, directionToTarget.x); // Angle in radians around the Y-axis

        if (distance > comfortableRange)
        {
            // Move toward predicted target but not too close
            Vector3 direction = (predictedTarget - transform.position).normalized;
            Vector3 stopShortPosition = predictedTarget - direction * comfortableRange;
            yield return UseMoveAction(stopShortPosition); // No yield needed here
        }
        else
        {
            // If within comfortable range, start circling around the target
            Vector3 circleCenter = targetFungal.transform.position;
            float circleRadius = comfortableRange / 2; // Adjust radius based on desired circle size

            // Update angle over time, starting from the initial angle
            float angle = initialAngle + (Time.time * 30f); // 30f adjusts the speed of rotation
            Vector3 offset = new Vector3(Mathf.Cos(angle) * circleRadius, 0f, Mathf.Sin(angle) * circleRadius);
            Vector3 circularPosition = circleCenter + offset;

            yield return UseMoveAction(circularPosition); // Move to a circular position around the target

            if (timeInSlingRange >= requiredTimeInRange)
            {
                // Use NavMeshAgent.Raycast to check if there is a clear path
                if (fishPickup.Fish.UseTrajectory || !agent.Raycast(targetFungal.transform.position, out NavMeshHit hit))
                {
                    fishPickup.Sling(predictedTarget);
                }
            }
        }
        yield break;
    }


    private void SetState(FungalState state)
    {
        currentState = state;

        switch (state)
        {
            case FungalState.THROW_FISH:
                timeInSlingRange = 0f;
                break;
        }
    }
}
