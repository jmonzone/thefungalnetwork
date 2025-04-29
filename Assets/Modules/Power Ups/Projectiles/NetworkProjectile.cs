using Unity.Netcode;

public class NetworkProjectile : NetworkBehaviour
{
    private Projectile projectile;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        projectile = GetComponent<Projectile>();

    }

    private void Update()
    {
        if (IsOwner && !projectile.InMotion)
        {
            transform.position = projectile.Fungal.transform.position;
        }
    }
}
