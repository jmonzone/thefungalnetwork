using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class FishPickup : NetworkBehaviour
{
    public Fish Fish { get; private set; }
    private NetworkFungal fungal;
    private Fish requestedFish;

    public event UnityAction OnFishChanged;
    public event UnityAction OnFishReleased;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        fungal = GetComponent<NetworkFungal>();
        fungal.OnDeath += _ => Fungal_OnDeath();

        requestedFish = null;
    }

    private void Fungal_OnDeath()
    {
        if (Fish)
        {
            Fish.ReturnToRadialMovement();
            RemoveFish();
        }
    }

    private void Update()
    {
        if (IsOwner && !Fish && !fungal.IsDead && !requestedFish) TryPickUpFish();
    }

    private void TryPickUpFish()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f); // Small detection radius

        foreach (Collider hit in hits)
        {
            var fish = hit.GetComponentInParent<Fish>();
            if (fish != null && !fish.IsPickedUp.Value)
            {
                requestedFish = fish;
                fish.OnPickUpRequest += Fish_OnPickUpRequest;
                fish.RequestPickUpServerRpc(NetworkObjectId);
                break;
            }
        }
    }

    private void Fish_OnPickUpRequest(bool success)
    {
        requestedFish.OnPickUpRequest -= Fish_OnPickUpRequest;

        if (success)
        {
            var networkPufferfish = requestedFish.GetComponent<Pufferfish>(); // Ensure it's the correct one
            if (networkPufferfish != null)
            {
                networkPufferfish.OnMaxTemperReached += NetworkPufferfish_OnMaxTemperReached;
                networkPufferfish.StartTemperServerRpc();
            }

            Fish = requestedFish;

            fungal.AddToScoreServerRpc(new ScoreEventArgs
            {
                value = Fish.Score,
                position = Fish.transform.position,
                label = "Catch"
            });
            OnFishChanged?.Invoke();
        }

        requestedFish = null;
    }

    public void Sling(Vector3 targetPosition)
    {
        if (Fish)
        {
            Fish.Throw(targetPosition);
            RemoveFish();
        }
    }

    private void RemoveFish()
    {
        var networkPufferfish = Fish.GetComponent<Pufferfish>(); // Ensure it's the correct one
        if (networkPufferfish != null)
        {
            networkPufferfish.OnMaxTemperReached -= NetworkPufferfish_OnMaxTemperReached;
        }

        Fish = null;
        OnFishChanged?.Invoke();
    }

    private void NetworkPufferfish_OnMaxTemperReached()
    {
        RemoveFish();
        OnFishReleased?.Invoke();
    }
}
