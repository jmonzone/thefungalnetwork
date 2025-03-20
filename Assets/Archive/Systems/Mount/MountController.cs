using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class MountController : NetworkBehaviour
{
    [SerializeField] private float mountHeight;

    private MovementController movement;
    private Mountable mountable;

    public NetworkVariable<bool> HasMount = new NetworkVariable<bool>(false);

    public Vector3 Direction => movement.Direction;
    public MovementController Movement => movement;
    public Mountable Mountable => mountable;

    public event UnityAction OnMounted;
    public event UnityAction OnUnmounted;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
        movement.OnJump += Unmount;
    }

    private void Update()
    {
        if (IsOwner && mountable)
        {
            transform.position = mountable.transform.position + Vector3.up * mountHeight;

            if (mountable.Movement.Direction.magnitude > 0)
            {
                transform.forward = mountable.Movement.Direction;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsOwner && !HasMount.Value)
        {
            var mount = other.GetComponentInParent<Mountable>();
            Mount(mount);
        }
    }

    public void Mount(Mountable mount)
    {
        if (mount && !mount.IsMounted.Value)
        {
            this.mountable = mount;
            mount.MountServerRpc(OwnerClientId, NetworkObjectId);
            MountServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void MountServerRpc()
    {
        HasMount.Value = true;
        OnMountClientRpc();
    }

    [ClientRpc]
    private void OnMountClientRpc()
    {
        if (IsOwner) OnMounted?.Invoke();
    }

    public void Unmount()
    {
        Debug.Log("unmounting");
        if (mountable) mountable.UnmountServerRpc();
        mountable = null;
        UnmountServerRpc();

    }

    [ServerRpc(RequireOwnership = false)]
    public void UnmountServerRpc()
    {
        HasMount.Value = false;
        OnUnmountClientRpc();
    }

    [ClientRpc]
    private void OnUnmountClientRpc()
    {
        mountable = null;
        if (IsOwner) OnUnmounted?.Invoke();
    }
}
