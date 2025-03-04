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

    public event UnityAction OnPrepareThrow;
    public event UnityAction OnPickup;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        movement = GetComponent<Movement>();
        throwFish = GetComponent<ThrowFish>();
    }

    public void Catch(Transform bobber)
    {
        movement.SetSpeed(10);
        movement.Follow(bobber);
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
            movement.SetSpeed(10);
            movement.Follow(playerReference.Movement.transform);
            OnPickup?.Invoke();  // Trigger pickup event only on the owning client
        }
    }

    public void ReturnToRadialMovement()
    {
        Debug.Log("ReturnToRadialMovement");

        StartCoroutine(RespawnRoutine());
    }

    private Vector3 originalScale;

    private IEnumerator RespawnRoutine()
    {
        originalScale = transform.GetChild(0).localScale;

        yield return new WaitForSeconds(1f);

        yield return ScaleOverTime(1f, 1f, 0f); // Shrink over 1 second

        yield return new WaitForSeconds(1f);

        transform.position = movement.CircleCenter;

        yield return ScaleOverTime(1f, 0f, 1f); // Grow back over 1 second

        movement.SetSpeed(swimSpeed);
        movement.StartRadialMovement(true);
        OnRespawnServerRpc();
    }

    // Generalized scaling coroutine
    private IEnumerator ScaleOverTime(float duration, float startScale, float endScale)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scaleFactor = Mathf.Lerp(startScale, endScale, elapsed / duration);
            transform.GetChild(0).localScale = originalScale * scaleFactor;
            yield return null;
        }

        transform.GetChild(0).localScale = originalScale * endScale; // Ensure final scale
    }


    [ServerRpc(RequireOwnership = false)]
    public void OnRespawnServerRpc()
    {
        IsPickedUp.Value = false;
    }

}