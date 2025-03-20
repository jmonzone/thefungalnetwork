using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PufferballController : NetworkBehaviour
{
    public Rigidbody Rigidbody { get; private set; }
    public NetworkTransform Transform { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Transform = GetComponent<NetworkTransform>();
        Spawn();
    }

    public void Spawn()
    {
        transform.position = new Vector3(0, 3, 0);
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.rotation = Quaternion.identity;
        gameObject.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void LaunchServerRpc(Vector3 direction)
    {
        TogglePhysicsClientRpc(true);
        LaunchClientRpc(direction);
    }

    [ClientRpc()]
    private void LaunchClientRpc(Vector3 direction)
    {
        if (IsOwner) Rigidbody.AddForce(1000f * direction);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TogglePhysicsServerRpc(bool value)
    {
        TogglePhysicsClientRpc(value);
    }

    [ClientRpc]
    private void TogglePhysicsClientRpc(bool value)
    {
        Rigidbody.isKinematic = !value;
        Rigidbody.useGravity = value;
    }

    // Server handles the actual movement
    [ServerRpc(RequireOwnership = false)]
    public void MoveObjectServerRpc(Vector3 newPosition)
    {
        // Broadcast the position to all clients
        MoveObjectClientRpc(newPosition);
    }

    // Sync the new position to all clients
    [ClientRpc]
    private void MoveObjectClientRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
