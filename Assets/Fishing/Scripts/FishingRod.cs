using System.Collections;
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
    [SerializeField] private bool scareFish = false;
    [SerializeField] private float launchSpeed = 0.5f;
    [SerializeField] private float launchAngle = 45f;

    [SerializeField] private float dragDistance = 7.5f;
    [SerializeField] private float dragSpeed = 3f;
    [SerializeField] private float launchThreshold = 100f;
    [SerializeField] private float launchScalar = 0.01f;

    private FishingRodState currentState;
    private Vector3 startPosition;
    private FishController targetFish;
    private Camera mainCamera;

    public event UnityAction<FishController> OnFishCaught;
    public event UnityAction<FishController> OnReeledIn;

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
        fishingLineRenderer.enabled = true;

        mainCamera = Camera.main;
    }

    private void Update()
    {
        fishingLineRenderer.SetPosition(0, fishingLineAnchor.position);
        fishingLineRenderer.SetPosition(1, bob.transform.position);

        if (currentState == FishingRodState.IN_WATER && !targetFish)
        {
            var fish = FishInRadius.FirstOrDefault();

            if (fish)
            {
                targetFish = fish;
                targetFish.Attract(bob.transform);
                targetFish.OnCaught += () =>
                {
                    OnFishCaught?.Invoke(targetFish);
                    ReelIn();
                };
            }

        }
    }

    public void Use()
    {
        if (currentState == FishingRodState.IDLE) StartCoroutine(CastOut());
        else if (currentState == FishingRodState.IN_WATER && (!targetFish || !targetFish.IsTreasure)) ReelIn();
    }

    private IEnumerator CastOut()
    {
        Debug.Log("starting cast");
        bob.Speed = dragSpeed;

        var inputDireciton = Vector3.zero;
        var startPosition = Input.mousePosition;

        while (Input.GetMouseButton(0))
        {
            yield return new WaitForFixedUpdate();
            inputDireciton = Input.mousePosition - startPosition;

            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            var targetPosition = ray.origin + ray.direction * dragDistance;
            bob.SetTargetPosition(targetPosition);
        }

        if (inputDireciton.magnitude < launchThreshold)
        {
            bob.SetTargetPosition(this.startPosition);
        }
        else
        {
            if (inputDireciton.magnitude < 50) inputDireciton = inputDireciton.normalized * 50;
            else if (inputDireciton.magnitude > 400) inputDireciton = inputDireciton.normalized * 400;

            bob.PersistDirection = true;
            var rotatedVector = Quaternion.Euler(launchAngle, 0, 0) * inputDireciton;
            rotatedVector.x *= 0.75f;
            bob.Launch(rotatedVector * launchScalar);
            SetCurrentState(FishingRodState.CASTING);
        }
    }

    private void ReelIn()
    {
        if (targetFish && !targetFish.IsCaught) targetFish.Scare(bob.transform.position);
        bob.PersistDirection = false;
        bob.Speed = 6f;
        bob.normalizeSpeed = false;
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
                    OnReeledIn?.Invoke(targetFish);
                    targetFish = null;
                }
                break;
            case FishingRodState.IN_WATER:
                catchRadiusIndicator.transform.position = bob.transform.position;
                catchRadiusIndicator.transform.localScale = catchRadius * 2f * Vector3.one;
                if (scareFish)
                {
                    foreach (var fish in FishInRadius)
                    {
                        fish.Scare(bob.transform.position);
                    }
                }
                break;
        }
    }

    private IEnumerable<FishController> FishInRadius
    {
        get
        {
            var catchPosition = bob.transform.position;
            var colliders = Physics.OverlapSphere(catchPosition, catchRadius, catchLayer);

            if (colliders.Length > 0)
            {
                var sortedColliders = colliders.OrderBy(collider =>
                {
                    return Vector3.Distance(collider.ClosestPoint(catchPosition), catchPosition);
                });

                foreach (var collider in sortedColliders)
                {
                    var fish = collider.GetComponentInParent<FishController>();

                    if (fish && fish.IsCatchable)
                    {
                        yield return fish;
                    }
                }
            }
        }
        
    }
}
