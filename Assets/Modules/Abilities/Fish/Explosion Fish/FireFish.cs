using UnityEngine;

public class FireFish : MonoBehaviour
{
    [SerializeField] private float minExplosionRadius = 3f;
    [SerializeField] private float maxExplosionRadius = 3f;
    [SerializeField] private float minExplosionDamage = 1f;
    [SerializeField] private float maxExplosionDamage = 3f;

    private FishController fish;
    private Movement movement;
    private PufferfishExplosion pufferfishExplosion;
    private PufferfishTemper pufferfishTemper;

    public float ExplosionRadius => pufferfishTemper.Temper * (maxExplosionRadius - minExplosionRadius) + minExplosionRadius;

    private void Awake()
    {
        fish = GetComponent<FishController>();
        movement = GetComponent<Movement>();
        pufferfishExplosion = GetComponent<PufferfishExplosion>();
        pufferfishTemper = GetComponent<PufferfishTemper>();
    }

    private void Start()
    {
        fish.ThrowFish.OnThrowComplete += OnThrowComplete;
        pufferfishExplosion.OnExplodeComplete += PufferfishExplosion_OnExplodeComplete;
    }

    private void PufferfishExplosion_OnExplodeComplete()
    {
        fish.Respawn();
        pufferfishTemper.StopTimer();
    }

    private void Update()
    {
        fish.ThrowFish.SetRadius(ExplosionRadius);
    }

    private void OnThrowComplete()
    {
        movement.Stop();
        pufferfishExplosion.StartExplosionAnimation(ExplosionRadius);
        float damage = pufferfishTemper.Temper * (maxExplosionDamage - minExplosionDamage) + minExplosionDamage;
        pufferfishExplosion.EnableDamage(damage);
    }
}
