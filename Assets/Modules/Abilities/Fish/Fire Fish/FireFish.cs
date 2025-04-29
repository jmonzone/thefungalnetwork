using UnityEngine;

public class FireFish : MonoBehaviour
{
    [SerializeField] private float minExplosionRadius = 3f;
    [SerializeField] private float maxExplosionRadius = 3f;
    [SerializeField] private float minExplosionDamage = 1f;
    [SerializeField] private float maxExplosionDamage = 3f;

    private FishController fish;
    private PufferfishExplosion pufferfishExplosion;
    private PufferfishTemper pufferfishTemper;

    public float ExplosionRadius => pufferfishTemper.Temper * (maxExplosionRadius - minExplosionRadius) + minExplosionRadius;

    private void Awake()
    {
        fish = GetComponent<FishController>();
        pufferfishExplosion = GetComponent<PufferfishExplosion>();
        pufferfishTemper = GetComponent<PufferfishTemper>();
    }

    private void Start()
    {
        fish.ThrowFish.OnTrajectoryComplete += StartExplosion;
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

    private void StartExplosion()
    {
        ShowExplosionAnimation();
        float damage = pufferfishTemper.Temper * (maxExplosionDamage - minExplosionDamage) + minExplosionDamage;
        pufferfishExplosion.EnableDamage(damage);
    }

    public void ShowExplosionAnimation()
    {
        pufferfishExplosion.StartExplosionAnimation(ExplosionRadius);
    }
}
