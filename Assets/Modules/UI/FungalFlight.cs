using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using UnityEngine;
using UnityEngine.Events;

public class FungalFlight : Ability
{
    [SerializeField] private float flightRange = 4f;
    [SerializeField] private float flightSpeed = 10f;

    public override Vector3 DefaultTargetPosition => transform.position + transform.forward * flightRange;

    public override bool UseTrajectory => true;

    public override float Range => flightRange;

    private NetworkFungal fungal;
    private Movement movement;
    private ClientNetworkTransform networkTransform;

    public event UnityAction OnFlightStart;
    public event UnityAction OnFlightComplete;

    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<Movement>();
        fungal = GetComponent<NetworkFungal>();
        networkTransform = GetComponent<ClientNetworkTransform>();
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        OnFlightStart?.Invoke();

        void OnDestinationReached()
        {
            movement.SetSpeed(fungal.BaseSpeed);
            movement.OnDestinationReached -= OnDestinationReached;

            networkTransform.Interpolate = true;
            OnFlightComplete?.Invoke();
        }

        networkTransform.Interpolate = false;
        movement.OnDestinationReached += OnDestinationReached;

        movement.SetSpeed(flightSpeed);
        movement.SetTrajectoryMovement(targetPosition);

        StartCoroutine(Cooldown.StartCooldown());
    }
}
