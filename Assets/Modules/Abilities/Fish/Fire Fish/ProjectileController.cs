using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Movement movement;

    private void Awake()
    {
        movement = GetComponent<Movement>();
    }

    public void Initialize(Vector3 targetPosition)
    {
        movement.SetTrajectoryMovement(targetPosition);
        GetComponent<TelegraphTrajectory>().ShowIndicator(targetPosition, 1f);
    }
}
