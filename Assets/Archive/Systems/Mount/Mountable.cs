using Unity.Netcode;
using UnityEngine;

//todo: centralize with crocodile interaction
public class Mountable : NetworkBehaviour
{
    public NetworkVariable<bool> IsMounted = new NetworkVariable<bool>(false);

    private MovementController movement;
    private MountController mountController;

    public MovementController Movement => movement;
    public MountController MountController => mountController;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void MountServerRpc(ulong clientId, ulong networkObjectId)
    {
        NetworkObject.ChangeOwnership(clientId);
        IsMounted.Value = true;
        OnMountClientRpc(clientId, networkObjectId);
    }

    [ClientRpc]
    private void OnMountClientRpc(ulong clientId, ulong networkObjectId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                mountController = networkObject.GetComponent<MountController>();
            }
        }
    }

    public void Unmount()
    {
        Debug.Log("Unmounted");
        if (mountController) mountController.UnmountServerRpc();
        UnmountServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void UnmountServerRpc()
    {
        Debug.Log("UnmountServerRpc");

        NetworkObject.RemoveOwnership();
        IsMounted.Value = false;
        OnUnmountClientRpc();
    }

    [ClientRpc]
    public void OnUnmountClientRpc()
    {
        mountController = null;

        if (IsOwner) movement.Stop();
    }
}
