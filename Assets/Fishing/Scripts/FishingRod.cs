using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum FishingRodState
{
    IDLE,
    CASTING,
    IN_AIR,
    IN_WATER,
    ATTRACTING,
    REELING
}

// handles input for fishing rod controls
public class FishingRod : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FishingBobController bob;
    [SerializeField] private Transform catchIndicator;

    [Header("Configuration")]
    [SerializeField] private float catchRadius = 5f;
    [SerializeField] private float minDistance = 0.1f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float sensitivity = 0.1f;

    private Camera mainCamera;
    private FishingRodState currentState;
    private Vector3 startInputPosition;
    private float dragDistance;
    private FishController targetFish;

    private bool IsUsing => Input.GetMouseButtonDown(0) && !Utility.IsPointerOverUI;

    private void OnEnable()
    {
        SetState(FishingRodState.IDLE);

        mainCamera = Camera.main;

        Vector3 planeNormal = mainCamera.transform.forward; // The normal of the plane
        Vector3 planePoint = mainCamera.transform.position; // A point on the plane

        // Calculate the distance from the target position to the plane
        dragDistance = Vector3.Dot(planeNormal, bob.transform.position - planePoint);
    }

    private void SetState(FishingRodState state)
    {
        currentState = state;
        bob.SetState(state);
        //catchIndicator.gameObject.SetActive(state == FishingRodState.IN_WATER);

        switch (currentState)
        {
            case FishingRodState.IDLE:
                if (targetFish)
                {
                    targetFish.gameObject.SetActive(false);
                    targetFish = null;
                }
                break;
            case FishingRodState.CASTING:
                startInputPosition = Input.mousePosition;
                break;
            case FishingRodState.IN_WATER:
                catchIndicator.transform.position = bob.transform.position;
                catchIndicator.transform.localScale = 2 * catchRadius / transform.localScale.x * Vector3.one;
                break;
            case FishingRodState.ATTRACTING:
                targetFish = CatchableFish[0];
                targetFish.Attract(bob);
                break;
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case FishingRodState.IDLE:
                if (IsUsing) SetState(FishingRodState.CASTING);
                break;
            case FishingRodState.CASTING:
                CastBob();
                if (Input.GetMouseButtonUp(0)) LaunchBob();
                break;
            case FishingRodState.IN_AIR:
                if (bob.transform.position.y < 0) SetState(FishingRodState.IN_WATER);
                break;
            case FishingRodState.IN_WATER:
                if (IsUsing) SetState(FishingRodState.REELING);
                else if (CatchableFish.Count > 0) SetState(FishingRodState.ATTRACTING);
                break;
            case FishingRodState.ATTRACTING:
                if (targetFish.State == FishState.CAUGHT) SetState(FishingRodState.REELING);
                break;
            case FishingRodState.REELING:
                if (bob.IsReeledIn) SetState(FishingRodState.IDLE);
                break;
        }
    }

    private void CastBob()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        var targetPosition = ray.origin + ray.direction * dragDistance;
        bob.transform.position = targetPosition;
    }

    private void LaunchBob()
    {
        var throwDirection = (Input.mousePosition - startInputPosition) * sensitivity;
        if (throwDirection.y < 0 || throwDirection.magnitude < minDistance)
        {
            SetState(FishingRodState.IDLE);
        }
        else
        {
            var launchVelocity = throwDirection.y * mainCamera.transform.up + throwDirection.x * mainCamera.transform.right;
            var clampedVelocity = Vector3.ClampMagnitude(launchVelocity, maxDistance);
            bob.Rigidbody.velocity = clampedVelocity;
            SetState(FishingRodState.IN_AIR);
        }
    }

    private List<FishController> CatchableFish => bob.transform.OverlapSphere<FishController>(catchRadius);
}
