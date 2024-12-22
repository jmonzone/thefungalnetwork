using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class MountController : NetworkBehaviour
{
    [SerializeField] private float mountHeight;

    private MovementController movement;
    private Mountable mount;

    public NetworkVariable<bool> HasMount = new NetworkVariable<bool>(false);

    public Vector3 Direction => movement.Direction;
    public Mountable Mount => mount;

    public event UnityAction OnMounted;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
        movement.OnJump += Unmount;
    }

    private void Update()
    {
        if (IsOwner && mount)
        {
            transform.position = mount.transform.position + Vector3.up * mountHeight;

            if (mount.Movement.Direction.magnitude > 0)
            {
                transform.forward = mount.Movement.Direction;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsOwner && !HasMount.Value)
        {
            var mount = other.GetComponentInParent<Mountable>();

            if (mount && !mount.IsMounted.Value)
            {
                this.mount = mount;
                mount.MountServerRpc(OwnerClientId, NetworkObjectId);
                MountServerRpc(OwnerClientId);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MountServerRpc(ulong clientId)
    {
        HasMount.Value = true;
        MounterClientRpc(clientId);
    }

    [ClientRpc]
    private void MounterClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            OnMounted?.Invoke();
            //mountedController = controller.Movement;
            //controller.SetMovement(movement);
            //mountedController.GetComponent<ProximityAction>().SetInteractable(false);
        }
    }

    private void Unmount()
    {
        UnmountServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnmountServerRpc(ulong clientId)
    {
        HasMount.Value = false;
        UnmountClientRpc(clientId);
    }

    [ClientRpc]
    private void UnmountClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            mount = null;
        }
    }
}
