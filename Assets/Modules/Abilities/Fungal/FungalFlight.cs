using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/Ability/Flight")]
public class FungalFlight : DirectionalAbility, IMovementAbility
{
    [SerializeField] private float flightRange = 4f;
    [SerializeField] private float flightSpeed = 10f;

    public override Vector3 DefaultTargetPosition => Fungal.transform.position + Fungal.transform.forward * flightRange;

    public override bool UseTrajectory => true;

    public override float Range => flightRange;
    public bool IgnoreObstacles => true;

    private Movement Movement => Fungal.Movement;


    protected override void OnAbilityCasted(Vector3 targetPosition)
    {
        void OnDestinationReached()
        {
            //Debug.Log("OnDestinationReached");
            Movement.SetSpeed(Fungal.BaseSpeed);
            Movement.OnDestinationReached -= OnDestinationReached;

            CompleteAbility();
        }

        //Debug.Log($"CastAbility {targetPosition}");

        Movement.OnDestinationReached += OnDestinationReached;

        Movement.SetSpeed(flightSpeed);
        Movement.SetTrajectoryMovement(targetPosition);
    }
}
