using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/Power Ups/New Projectile Ability")]
public class ProjectileAbility : DirectionalAbility
{
    [SerializeField] private Movement projectilePrefab;
    [SerializeField] private float range;

    [SerializeField] private bool useTrajectory;

    public override float Range => range;
    public override bool UseTrajectory => useTrajectory;

    public List<Projectile> Projectiles { get; private set; } = new List<Projectile>();

    private int projectileIndex = -1;

    public override void Initialize(FungalController fungal)
    {
        base.Initialize(fungal);

        for(var i = 0; i < 2; i++)
        {
            Fungal.RequestSpawnObject(projectilePrefab.gameObject, fungal.transform.position);
        }
    }

    public override void CastAbility(Vector3 targetPosition)
    {   
        base.CastAbility();

        projectileIndex = (projectileIndex + 1) % Projectiles.Count;

        Projectiles[projectileIndex].ShootProjectile(Fungal.transform.position, targetPosition);
    }

    public void AssignProjectile(Projectile projectile)
    {
        Projectiles.Add(projectile);
        projectile.Initialize(Fungal, useTrajectory);
    }
}
