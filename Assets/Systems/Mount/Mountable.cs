using Unity.Netcode;

//todo: centralize with crocodile interaction
public class Mountable : NetworkBehaviour
{
    public NetworkVariable<bool> IsMounted = new NetworkVariable<bool>(false);

    private MovementController mountMovement;

    public MovementController Movement => mountMovement;

    private void Awake()
    {
        mountMovement = GetComponent<MovementController>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void MountServerRpc(ulong clientId, ulong networkObjectId)
    {
        NetworkObject.ChangeOwnership(clientId);
        IsMounted.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UnmountServerRpc()
    {
        var owner = NetworkObject.OwnerClientId;
        NetworkObject.RemoveOwnership();
        IsMounted.Value = false;
        UnmountClientRpc(owner);
    }

    [ClientRpc]
    public void UnmountClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            mountMovement.Stop();
        }
    }
}
