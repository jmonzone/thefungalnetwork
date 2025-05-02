using UnityEngine;
using UnityEngine.Events;

public interface IProjectileBehavior
{
    void Shoot(Vector3 startPosition, Vector3 targetPosition);
    void Initialize(Projectile projectile);
}

public class Projectile : MonoBehaviour, ITrajectory
{
    [SerializeField] private float damage;
    [SerializeField] private float hitStun = 0.5f;
    [SerializeField] private float radius;

    [HideInInspector] public HitDetector hitDetector;
    private IProjectileBehavior behavior;

    public FungalController Fungal { get; private set; }
    public Movement Movement { get; private set; }
    public bool UseTrajectory { get; private set; }
    public bool InMotion { get; set; }

    public float Damage => damage;
    public float HitStun => hitStun;
    float ITrajectory.Radius => radius;

    public float Radius => radius;

    public event UnityAction<Vector3> OnTrajectoryStart;
    public event UnityAction OnTrajectoryComplete;

    private void Awake()
    {
        hitDetector = GetComponent<HitDetector>();
        Movement = GetComponent<Movement>();
        behavior = GetComponent<IProjectileBehavior>();
        behavior?.Initialize(this);
    }

    public void Initialize(FungalController fungal, bool useTrajectory)
    {
        Fungal = fungal;
        UseTrajectory = useTrajectory;
        transform.GetChild(0).localScale = Vector3.zero;
    }

    public void ShootProjectile(Vector3 startPosition, Vector3 targetPosition)
    {
        transform.position = startPosition;

        behavior?.Shoot(startPosition, targetPosition);
        OnTrajectoryStart?.Invoke(targetPosition);
    }

    public void CompleteTrajectory()
    {
        OnTrajectoryComplete?.Invoke();
    }
}
