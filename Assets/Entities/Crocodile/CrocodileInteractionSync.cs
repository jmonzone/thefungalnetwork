using Unity.Netcode;

public class CrocodileInteractionSync : NetworkBehaviour
{
    private CrocodileInteraction crocodileInteraction;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        crocodileInteraction = GetComponent<CrocodileInteraction>();
        crocodileInteraction.OnMounted += () =>
        {
            SyncMountServerRpc(NetworkManager.Singleton.LocalClientId);
        };

        crocodileInteraction.OnUnmounted += () =>
        {
            SyncUnmountServerRpc(NetworkManager.Singleton.LocalClientId);
        };
    }

    [ServerRpc(RequireOwnership=false)]
    public void SyncMountServerRpc(ulong clientId)
    {
        NetworkObject.ChangeOwnership(clientId);
        SyncMountClientRpc(clientId);
    }

    [ClientRpc]
    public void SyncMountClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) return;
        crocodileInteraction.SyncMount();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SyncUnmountServerRpc(ulong clientId)
    {
        NetworkObject.RemoveOwnership();
        SyncUnmountClientRpc(clientId);
    }

    [ClientRpc]
    public void SyncUnmountClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) return;
        crocodileInteraction.SyncUnmount();
    }
}
