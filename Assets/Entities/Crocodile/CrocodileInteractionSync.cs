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
            SyncMountClientRpc(NetworkManager.Singleton.LocalClientId);
        };
    }

    [ClientRpc]
    public void SyncMountClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) return;
        crocodileInteraction.Restore();
    }
}
