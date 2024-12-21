using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MushroomObjectiveSync : NetworkBehaviour
{
    private MushroomObjective objective;
    private void Awake()
    {
        objective = GetComponent<MushroomObjective>();
        objective.OnMounted += () =>
        {
            SyncMountServerRpc(NetworkManager.Singleton.LocalClientId);
        };

        objective.OnUnmounted += () =>
        {
            SyncUnmountServerRpc(NetworkManager.Singleton.LocalClientId);

        };
    }

    [ServerRpc(RequireOwnership = false)]
    public void SyncMountServerRpc(ulong clientId)
    {
        NetworkObject.ChangeOwnership(clientId);
        SyncMountClientRpc(clientId);
    }

    [ClientRpc]
    public void SyncMountClientRpc(ulong clientId)
    {
        objective.enabled = NetworkManager.Singleton.LocalClientId == clientId;
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
        objective.enabled = true;
    }
}
