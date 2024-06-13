using UnityEngine;

public class FishingRod : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody bob;
    [SerializeField] private float minDistance = 0.1f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float gravity = 0.1f;

    private Camera mainCamera;

    private FishingRodState currentState;
    private Vector3 startInputPosition;
    private Vector3 startBobPosition;
    private float dragDistance;

    private enum FishingRodState
    {
        IDLE,
        CASTING,
        IN_AIR,
        IN_WATER,
        REELING
    }

    private void Awake()
    {
        mainCamera = Camera.main;
        startBobPosition = bob.transform.position;

        Vector3 planeNormal = mainCamera.transform.forward; // The normal of the plane
        Vector3 planePoint = mainCamera.transform.position; // A point on the plane
        Vector3 targetPosition = bob.transform.position; // The position of the target

        // Calculate the distance from the target position to the plane
        dragDistance = Vector3.Dot(planeNormal, targetPosition - planePoint);
    }

    private void OnEnable()
    {
        SetState(FishingRodState.IDLE);
    }

    private void SetState(FishingRodState state)
    {
        currentState = state;
        bob.useGravity = state == FishingRodState.IN_AIR;

        switch (currentState)
        {
            case FishingRodState.IDLE:
                bob.useGravity = false;
                bob.velocity = Vector3.zero;
                bob.MovePosition(startBobPosition);
                break;
            case FishingRodState.CASTING:
                startInputPosition = Input.mousePosition;
                break;
            case FishingRodState.IN_WATER:
                var position = bob.transform.position;
                position.y = 0;
                bob.velocity = Vector3.zero;
                bob.MovePosition(position);
                break;
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case FishingRodState.IDLE:
            case FishingRodState.IN_WATER:
                if (Input.GetMouseButtonDown(0) && !Utility.IsPointerOverUI)
                {
                    SetState(FishingRodState.CASTING);
                }
                break;
            case FishingRodState.CASTING:
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                var targetPosition = ray.origin + ray.direction * dragDistance;
                bob.MovePosition(targetPosition);

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
                        bob.velocity = Vector3.ClampMagnitude(launchVelocity, maxDistance);
                        SetState(FishingRodState.IN_AIR);
                    }
                }
                break;
            case FishingRodState.IN_AIR:
                if (bob.transform.position.y <= 0)
                {
                    SetState(FishingRodState.IN_WATER);
                }
                else
                {
                    bob.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
                }
                break;
        }
    }
}
