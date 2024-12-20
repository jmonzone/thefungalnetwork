using Unity.Netcode;

public class CrocodileInteractionSync : NetworkBehaviour
{
    private CrocodileInteraction crocodileInteraction;
    private CrocodileCharge crocodileCharge;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        crocodileCharge = GetComponent<CrocodileCharge>();
        crocodileCharge.enabled = IsOwner;

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
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            crocodileCharge.enabled = true;
            return;
        }
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
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            crocodileCharge.enabled = false;
            return;
        }
        crocodileInteraction.SyncUnmount();
    }
}
