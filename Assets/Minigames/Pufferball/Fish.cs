using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Fish : NetworkBehaviour
{
    [SerializeField] private PlayerReference playerReference;

    private Movement movement;
    private ThrowFish throwFish;

    public event UnityAction OnPickup;

    private void Awake()
    {
        movement = GetComponent<Movement>();

        throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowComplete += InvokeReturnToRadialMovement;
    }

    public void InvokeReturnToRadialMovement()
    {
        Invoke(nameof(ReturnToRadialMovement), 1f);
    }

    private void ReturnToRadialMovement()
    {
        movement.SetSpeed(5);
        movement.StartRadialMovement(true);
    }

    public void Catch(Transform bobber)
    {
        movement.SetSpeed(10);
        movement.Follow(bobber);
        RequestCatchServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void PickUp()
    {
        movement.Follow(playerReference.Movement.transform);
        RequestPickUpServerRpc(NetworkManager.Singleton.LocalClientId);
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
        OnPickup?.Invoke();
    }
}