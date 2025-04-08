using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/Ability/Flight")]
public class FungalFlight : Ability
{
    [SerializeField] private float flightRange = 4f;
    [SerializeField] private float flightSpeed = 10f;

    public override Vector3 DefaultTargetPosition => fungal.transform.position + fungal.transform.forward * flightRange;

    public override bool UseTrajectory => true;

    public override float Range => flightRange;

    private Movement Movement => fungal.Movement;
    private ClientNetworkTransform networkTransform;

    public override void Initialize(NetworkFungal fungal)
    {
        base.Initialize(fungal);
        networkTransform = fungal.GetComponent<ClientNetworkTransform>();
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        base.CastAbility(targetPosition);

        void OnDestinationReached()
        {
            Debug.Log("OnDestinationReached");
            Movement.SetSpeed(fungal.BaseSpeed);
            Movement.OnDestinationReached -= OnDestinationReached;

            networkTransform.Interpolate = true;
            CompleteAbility();
        }

        Debug.Log($"CastAbility {targetPosition}");

        networkTransform.Interpolate = false;
        Movement.OnDestinationReached += OnDestinationReached;

        Movement.SetSpeed(flightSpeed);
        Movement.SetTrajectoryMovement(targetPosition);

        fungal.StartCoroutine(Cooldown.StartCooldown());
    }
}
