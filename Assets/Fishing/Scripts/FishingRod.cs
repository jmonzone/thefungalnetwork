using System.Collections;
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

    private Vector3 startBobPosition;
    private float dragDistance;


    //[SerializeField] private GameObject catchRadiusIndicator;
    //[SerializeField] private Transform fishingLineAnchor;

    //private LineRenderer fishingLineRenderer;

    //[SerializeField] private LayerMask oceanLayer;
    //[SerializeField] private LayerMask catchLayer;
    //[SerializeField] private float catchRadius = 5f;
    //[SerializeField] private bool scareFish = false;
    //[SerializeField] private float launchSpeed = 0.5f;
    //[SerializeField] private float launchAngle = 45f;

    //[SerializeField] private float dragSpeed = 3f;
    //[SerializeField] private float launchThreshold = 100f;
    //[SerializeField] private float launchScalar = 0.01f;


    //private FishController targetFish;

    //public event UnityAction<FishController> OnFishCaught;
    //public event UnityAction<FishController> OnReeledIn;

    private FishingRodState currentState;
    private Vector3 startInputPosition;

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
        startBobPosition = bob.transform.position;
        mainCamera = Camera.main;
        SetState(FishingRodState.IDLE);

        Vector3 planeNormal = mainCamera.transform.forward; // The normal of the plane
        Vector3 planePoint = mainCamera.transform.position; // A point on the plane
        Vector3 targetPosition = bob.transform.position; // The position of the target

        // Calculate the distance from the target position to the plane
        dragDistance = Vector3.Dot(planeNormal, targetPosition - planePoint);

        Debug.Log(dragDistance);
        //bob.OnDestinationReached += () =>
        //{
        //    if (currentState == FishingRodState.CASTING) SetCurrentState(FishingRodState.IN_WATER);
        //    else if (currentState == FishingRodState.REELING) SetCurrentState(FishingRodState.IDLE);
        //};

        //fishingLineRenderer = GetComponent<LineRenderer>();
        //fishingLineRenderer.enabled = true;

    }

    private void Update()
    {
        switch (currentState)
        {
            case FishingRodState.CASTING:
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                var targetPosition = ray.origin + ray.direction * dragDistance;
                bob.MovePosition(targetPosition);

                if (Input.GetMouseButtonUp(0))
                {
                    var throwDirection = Input.mousePosition - startInputPosition;
                    var launchVelocity = throwDirection.y * mainCamera.transform.up + throwDirection.x * mainCamera.transform.right;
                    launchVelocity = Vector3.ClampMagnitude(launchVelocity * sensitivity, maxDistance);

                    if (launchVelocity.magnitude > minDistance)
                    {
                        bob.velocity = launchVelocity;
                        SetState(FishingRodState.IN_AIR);
                    }
                    else
                    {
                        SetState(FishingRodState.IDLE);
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

    public void Use()
    {
        SetState(FishingRodState.CASTING);
    }

    //private void Update()
    //{
    //    fishingLineRenderer.SetPosition(0, fishingLineAnchor.position);
    //    fishingLineRenderer.SetPosition(1, bob.transform.position);

    //    if (currentState == FishingRodState.IN_WATER && !targetFish)
    //    {
    //        var fish = FishInRadius.FirstOrDefault();

    //        if (fish)
    //        {
    //            targetFish = fish;
    //            targetFish.Attract(bob.transform);
    //            targetFish.OnCaught += () =>
    //            {
    //                OnFishCaught?.Invoke(targetFish);
    //                ReelIn();
    //            };
    //        }

    //    }
    //}



    //private void ReelIn()
    //{
    //    if (targetFish && !targetFish.IsCaught) targetFish.Scare(bob.transform.position);
    //    //bob.PersistDirection = false;
    //    //bob.Speed = 6f;
    //    //bob.normalizeSpeed = false;
    //    //bob.SetTargetPosition(startPosition);
    //    SetCurrentState(FishingRodState.REELING);
    //}

    //private void SetCurrentState(FishingRodState state)
    //{
    //    currentState = state;
    //    catchRadiusIndicator.SetActive(state == FishingRodState.IN_WATER);

    //    switch (state)
    //    {
    //        case FishingRodState.IDLE:
    //            if (targetFish)
    //            {
    //                targetFish.gameObject.SetActive(false);
    //                OnReeledIn?.Invoke(targetFish);
    //                targetFish = null;
    //            }
    //            break;
    //        case FishingRodState.IN_WATER:
    //            catchRadiusIndicator.transform.position = bob.transform.position;
    //            catchRadiusIndicator.transform.localScale = catchRadius * 2f * Vector3.one;
    //            if (scareFish)
    //            {
    //                foreach (var fish in FishInRadius)
    //                {
    //                    fish.Scare(bob.transform.position);
    //                }
    //            }
    //            break;
    //    }
    //}

    //private IEnumerable<FishController> FishInRadius
    //{
    //    get
    //    {
    //        var catchPosition = bob.transform.position;
    //        var colliders = Physics.OverlapSphere(catchPosition, catchRadius, catchLayer);

    //        if (colliders.Length > 0)
    //        {
    //            var sortedColliders = colliders.OrderBy(collider =>
    //            {
    //                return Vector3.Distance(collider.ClosestPoint(catchPosition), catchPosition);
    //            });

    //            foreach (var collider in sortedColliders)
    //            {
    //                var fish = collider.GetComponentInParent<FishController>();

    //                if (fish && fish.IsCatchable)
    //                {
    //                    yield return fish;
    //                }
    //            }
    //        }
    //    }

    //}
}
