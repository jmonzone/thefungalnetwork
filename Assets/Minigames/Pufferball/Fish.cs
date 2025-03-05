using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Fish : NetworkBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float swimSpeed = 1f;

    public Movement Movement { get; private set; }
    private ThrowFish throwFish;

    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(
        Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public event UnityAction OnPrepareThrow;
    public event UnityAction OnPickup;
    public event UnityAction OnRespawn;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Movement = GetComponent<Movement>();
        throwFish = GetComponent<ThrowFish>();

        if (IsServer)
        {
            networkPosition.Value = transform.position; // Set initial position on the server
        }
    }

    public void Catch(Transform bobber)
    {
        Movement.SetSpeed(10);
        Movement.Follow(bobber);
        RequestCatchServerRpc(NetworkManager.Singleton.LocalClientId);
    }


    public void PrepareThrow()
    {
        OnPrepareThrow?.Invoke();
    }

    public void Throw(Vector3 targetPosition)
    {
        throwFish.Throw(targetPosition);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestCatchServerRpc(ulong requestingClientId)
    {
        NetworkObject.ChangeOwnership(requestingClientId);
    }


    public NetworkVariable<bool> IsPickedUp = new NetworkVariable<bool>(false);

    public bool PickUp()
    {
        if (IsPickedUp.Value) return false;  // Return false if it's already picked up

        RequestPickUpServerRpc(NetworkManager.Singleton.LocalClientId);
        return true; // Return true if the pick-up was successful
    }


    [ServerRpc(RequireOwnership = false)]
    private void RequestPickUpServerRpc(ulong clientId)
    {
        Debug.Log($"RequestPickUpServerRpc {IsPickedUp.Value}");

        if (IsPickedUp.Value) return;  // Double check server-side to prevent race conditions.

        IsPickedUp.Value = true;
        NetworkObject.ChangeOwnership(clientId);
        OnPickupClientRpc(clientId);
    }

    [ClientRpc]
    private void OnPickupClientRpc(ulong clientId)
    {
        Debug.Log($"OnPickupClientRpc {NetworkManager.Singleton.LocalClientId == clientId}");

        // Update the local state after ownership change
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            Movement.SetSpeed(10);
            Movement.Follow(playerReference.Movement.transform);
            OnPickup?.Invoke();  // Trigger pickup event only on the owning client
        }
    }

    public void ReturnToRadialMovement()
    {
        Debug.Log("ReturnToRadialMovement");

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        transform.position = networkPosition.Value;

        yield return new WaitForSeconds(2f);

        yield return Movement.ScaleOverTime(1f, 0f, 1f); // Grow back over 1 second

        Movement.SetSpeed(swimSpeed);
        Movement.StartRadialMovement(networkPosition.Value, true);
        OnRespawnServerRpc();
        OnRespawn?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnRespawnServerRpc()
    {
        IsPickedUp.Value = false;
    }

}