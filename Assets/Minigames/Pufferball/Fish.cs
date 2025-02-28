using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Fish : NetworkBehaviour
{
    [SerializeField] private PlayerReference playerReference;

    private Movement movement;
    private ThrowFish throwFish;

    public event UnityAction OnPrepareThrow;
    public event UnityAction OnPickup;

    private void Awake()
    {
        movement = GetComponent<Movement>();

        throwFish = GetComponent<ThrowFish>();
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
        OnPickup?.Invoke();
    }

    public void ReturnToRadialMovement()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(1f);

        transform.position = movement.CircleCenter;
        movement.SetSpeed(3);
        movement.StartRadialMovement(true);
    }

}