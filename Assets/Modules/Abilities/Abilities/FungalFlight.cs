using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/Ability/Flight")]
public class FungalFlight : DirectionalAbility, IMovementAbility
{
    [SerializeField] private float flightRange = 4f;
    [SerializeField] private float flightSpeed = 10f;

    public override Vector3 DefaultTargetPosition => Fungal.transform.position + Fungal.transform.forward * flightRange;

    public override bool UseTrajectory => true;

    public override float Range => flightRange;

    private Movement Movement => Fungal.Movement;
    //private ClientNetworkTransform networkTransform;

    public override void Initialize(FungalController fungal)
    {
        base.Initialize(fungal);
        //networkTransform = fungal.GetComponent<ClientNetworkTransform>();
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        base.CastAbility(targetPosition);

        void OnDestinationReached()
        {
            Debug.Log("OnDestinationReached");
            Movement.SetSpeed(Fungal.BaseSpeed);
            Movement.OnDestinationReached -= OnDestinationReached;

            //networkTransform.Interpolate = true;
            CompleteAbility();
        }

        Debug.Log($"CastAbility {targetPosition}");

        //networkTransform.Interpolate = false;
        Movement.OnDestinationReached += OnDestinationReached;

        Movement.SetSpeed(flightSpeed);
        Movement.SetTrajectoryMovement(targetPosition);
    }
}
