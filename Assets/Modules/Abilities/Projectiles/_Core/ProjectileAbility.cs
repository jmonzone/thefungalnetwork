using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/Power Ups/New Projectile Ability")]
public class ProjectileAbility : DirectionalAbility
{
    [SerializeField] private Movement projectilePrefab;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float range = 3f;

    [SerializeField] private bool useMaxCount = false;
    [SerializeField] private int maxCount = 3;
    [SerializeField] private bool useTrajectory;

    public override float Range => range;
    public override bool UseTrajectory => useTrajectory;

    public List<Projectile> Projectiles { get; private set; } = new List<Projectile>();

    private int projectileIndex = -1;

    private int uses = 0;

    public override void Initialize(FungalController fungal, AbilitySlot index)
    {
        base.Initialize(fungal, index);

        var objectPoolCount = 2;
        var received = 0;

        void AssignProjectile(GameObject obj)
        {
            Debug.Log("AssignProjectile " + name);

            var projectile = obj.GetComponent<Projectile>();
            Projectiles.Add(projectile);
            projectile.Initialize(Fungal, useTrajectory);
            projectile.Movement.SetSpeed(speed);

            received++;
            if (received >= objectPoolCount)
            {
                Fungal.OnObjectHasSpawned -= AssignProjectile;
            }
        }

        Fungal.OnObjectHasSpawned += AssignProjectile;

        for (var i = 0; i < objectPoolCount; i++)
        {
            Fungal.RequestSpawnObject(projectilePrefab.gameObject, fungal.transform.position);
        }
    }

    public override void OnReassigned(AbilitySlot slot)
    {
        base.OnReassigned(slot);
        uses = 0;
    }

    protected override void OnAbilityCasted(Vector3 targetPosition)
    {
        projectileIndex = (projectileIndex + 1) % Projectiles.Count;

        Projectiles[projectileIndex].ShootProjectile(Fungal.transform.position, targetPosition);

        uses++;

        if (useMaxCount && uses >= maxCount)
        {
            Debug.Log("remove");
            RemoveAbility();
        }
    }
}
