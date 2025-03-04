using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Fish : NetworkBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float swimSpeed = 1f;

    private Movement movement;
    private ThrowFish throwFish;

    public bool CanPickUp { get; private set; }

    public event UnityAction OnPrepareThrow;
    public event UnityAction OnPickup;

    private void Awake()
    {
        movement = GetComponent<Movement>();

        throwFish = GetComponent<ThrowFish>();

        CanPickUp = true;
    }

    public void Catch(Transform bobber)
    {
        movement.SetSpeed(10);
        movement.Follow(bobber);
        RequestCatchServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void PickUp()
    {
        movement.SetSpeed(10);
        movement.Follow(playerReference.Movement.transform);
        RequestPickUpServerRpc(NetworkManager.Singleton.LocalClientId);
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

    [ServerRpc(RequireOwnership = false)]
    public void RequestPickUpServerRpc(ulong clientId)
    {
        NetworkObject.ChangeOwnership(clientId);
        OnPickupClientRpc();
        OnPickup?.Invoke();
    }

    [ClientRpc]
    private void OnPickupClientRpc()
    {
        CanPickUp = false;
    }

    public void ReturnToRadialMovement()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(1f);

        movement.SetSpeed(swimSpeed);
        movement.StartRadialMovement(true);
        OnRespawnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnRespawnServerRpc()
    {
        OnRespawnClientRpc();
    }

    [ClientRpc]
    private void OnRespawnClientRpc()
    {
        CanPickUp = true;
    }

}