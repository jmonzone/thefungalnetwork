using System.Collections.Generic;
using UnityEngine;

public enum FishingRodState
{
    IDLE,
    CASTING,
    IN_AIR,
    IN_WATER,
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

    private bool IsUsing => Input.GetMouseButtonDown(0) && !Utility.IsPointerOverUI;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        SetState(FishingRodState.IDLE);

        Vector3 planeNormal = mainCamera.transform.forward; // The normal of the plane
        Vector3 planePoint = mainCamera.transform.position; // A point on the plane

        // Calculate the distance from the target position to the plane
        dragDistance = Vector3.Dot(planeNormal, bob.transform.position - planePoint);
    }

    private void SetState(FishingRodState state)
    {
        currentState = state;
        bob.SetState(state);
        catchIndicator.gameObject.SetActive(state == FishingRodState.IN_WATER);

        switch (currentState)
        {
            case FishingRodState.CASTING:
                startInputPosition = Input.mousePosition;
                break;
            case FishingRodState.IN_WATER:
                catchIndicator.transform.position = bob.transform.position;
                catchIndicator.transform.localScale = Vector3.one * catchRadius;
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
            case FishingRodState.IN_WATER:
                if (IsUsing) SetState(FishingRodState.REELING);
                else
                {

                }
                break;
            case FishingRodState.CASTING:
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                var targetPosition = ray.origin + ray.direction * dragDistance;
                bob.Rigidbody.MovePosition(targetPosition);

                if (Input.GetMouseButtonUp(0))
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
                break;
            case FishingRodState.IN_AIR:
                if (bob.transform.position.y < 0) SetState(FishingRodState.IN_WATER);
                break;
            case FishingRodState.REELING:
                if (bob.IsReeledIn) SetState(FishingRodState.IDLE);
                break;
        }
    }
}
