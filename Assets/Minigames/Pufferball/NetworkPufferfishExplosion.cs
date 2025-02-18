using Unity.Netcode;

public class NetworkPufferfishExplosion : NetworkBehaviour
{
    private PufferfishExplosion pufferfishExplosion;

    private void Awake()
    {
        pufferfishExplosion = GetComponent<PufferfishExplosion>();
    }

    [ServerRpc]
    public void ExplodeServerRpc(float radius)
    {
        ExplodeClientRpc(radius);
    }

    [ClientRpc]
    private void ExplodeClientRpc(float radius)
    {
        pufferfishExplosion.Explode(radius);
    }
}
