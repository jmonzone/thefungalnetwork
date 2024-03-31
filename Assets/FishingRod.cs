using System.Collections.Generic;
using System.Linq;
using GURU.Entities;
using UnityEngine;
using UnityEngine.Events;

public class FishingRod : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private MovementController bob;
    [SerializeField] private GameObject catchRadiusIndicator;
    [SerializeField] private Transform fishingLineAnchor;

    private LineRenderer fishingLineRenderer;

    [SerializeField] private LayerMask oceanLayer;
    [SerializeField] private LayerMask catchLayer;
    [SerializeField] private float catchRadius = 5f;

    private Vector3 startPosition;
    private FishingRodState currentState;
    private Fish targetFish;

    public event UnityAction OnFishCaught;

    private enum FishingRodState
    {
        IDLE,
        CASTING,
        IN_WATER,
        REELING
    }

    private void Awake()
    {
        startPosition = bob.transform.position;
        bob.OnDestinationReached += () =>
        {
            if (currentState == FishingRodState.CASTING) SetCurrentState(FishingRodState.IN_WATER);
            else if (currentState == FishingRodState.REELING) SetCurrentState(FishingRodState.IDLE);
        };

        fishingLineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        fishingLineRenderer.SetPosition(0, fishingLineAnchor.position);
        fishingLineRenderer.SetPosition(1, bob.transform.position);

        if (currentState == FishingRodState.IN_WATER && !targetFish)
        {
            var fish = FishInRadius.FirstOrDefault();

            if (fish && fish.IsCatchable)
            {
                targetFish = fish;
                targetFish.Attract(bob.transform);
                targetFish.OnCaught += () => ReelIn();
            }

        }
    }

    public void Use()
    {
        if (currentState == FishingRodState.IDLE) CastOut();
        else if (currentState == FishingRodState.IN_WATER) ReelIn();
    }

    private void CastOut()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, oceanLayer))
        {
            var targetPosition = hit.point;
            targetPosition.y = 0;

            bob.SetTargetPosition(targetPosition);
            SetCurrentState(FishingRodState.CASTING);
        }
    }

    private void ReelIn()
    {
        bob.SetTargetPosition(startPosition);
        SetCurrentState(FishingRodState.REELING);
    }

    private void SetCurrentState(FishingRodState state)
    {
        currentState = state;
        catchRadiusIndicator.SetActive(state == FishingRodState.IN_WATER);

        switch (state)
        {
            case FishingRodState.IDLE:
                if (targetFish)
                {
                    targetFish.gameObject.SetActive(false);
                    OnFishCaught?.Invoke();

                    targetFish = null;
                }
                break;
            case FishingRodState.IN_WATER:
                catchRadiusIndicator.transform.localScale = catchRadius * 2f * Vector3.one;
                foreach(var fish in FishInRadius)
                {
                    fish.Scare(bob.transform.position);
                }
                break;
        }
    }

    private IEnumerable<Fish> FishInRadius
    {
        get
        {
            var colliders = Physics.OverlapSphere(bob.transform.position, catchRadius, catchLayer);
            if (colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    var fish = collider.GetComponentInParent<Fish>();

                    if (fish)
                    {
                        yield return fish;
                    }
                }
            }
        }
        
    }
}
