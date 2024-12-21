using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

//todo: centralize with crocodile interaction
public class MushroomObjective : NetworkBehaviour
{
    [SerializeField] private Controller controller;

    private NetworkVariable<bool> IsMounted = new NetworkVariable<bool>(false);
    private MovementController movement;
    private MovementController mountedController;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
        movement.OnJump += UnmountServerRpc;
    }

    private void Update()
    {
        if (mountedController)
        {
            mountedController.transform.position = transform.position + Vector3.up * 0.625f;
            mountedController.LookAt(controller.Movement.Direction);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsMounted.Value) return;
        if (mountedController) return;
            
        Rigidbody playerRb = collision.rigidbody;

        var movement = collision.gameObject.GetComponentInParent<MovementController>();

        // Ensure the player is the one colliding
        if (movement && movement == controller.Movement)
        {
            // Check if the player is falling onto the object
            if (playerRb.velocity.y < 0)
            {
                Debug.Log("Player landed on this object!");
                MountServerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void MountServerRpc(ulong clientId)
    {
        NetworkObject.ChangeOwnership(clientId);
        IsMounted.Value = true;
        MounterClientRpc(clientId);
    }

    [ClientRpc]
    public void MounterClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            mountedController = controller.Movement;
            controller.SetMovement(movement);
            mountedController.GetComponent<ProximityAction>().SetInteractable(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UnmountServerRpc()
    {
        NetworkObject.RemoveOwnership();
        IsMounted.Value = false;
        UnmountClientRpc(NetworkObject.OwnerClientId);
    }

    [ClientRpc]
    public void UnmountClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            controller.SetMovement(mountedController);
            mountedController.GetComponent<ProximityAction>().SetInteractable(true);
            movement.Stop();
            mountedController = null;
        }
    }
}
