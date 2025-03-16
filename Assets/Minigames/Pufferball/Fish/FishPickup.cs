using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class FishPickup : NetworkBehaviour
{
    public Fish Fish { get; private set; }
    private NetworkFungal fungal;

    public event UnityAction OnFishChanged;
    public event UnityAction OnFishReleased;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        fungal = GetComponent<NetworkFungal>();
        fungal.OnDeath += Fungal_OnHealthDepleted;
    }

    private void Fungal_OnHealthDepleted()
    {
        if (Fish)
        {
            Fish.ReturnToRadialMovement();
            RemoveFish();
        }
    }

    private void Update()
    {
        if (IsOwner && !Fish && !fungal.IsDead) DetectPufferfishHit();
    }

    private bool DetectPufferfishHit()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f); // Small detection radius

        foreach (Collider hit in hits)
        {
            var fish = hit.GetComponentInParent<Fish>();
            if (fish != null && !fish.IsPickedUp.Value)
            {
                var networkPufferfish = fish.GetComponent<Pufferfish>(); // Ensure it's the correct one
                if (networkPufferfish != null)
                {
                    networkPufferfish.OnMaxTemperReached += NetworkPufferfish_OnMaxTemperReached;
                }

                bool pickupSuccessful = fish.PickUp(); // This now returns whether the pickup was successful
                if (pickupSuccessful)
                {
                    Fish = fish;
                    OnFishChanged?.Invoke();
                    return true; // Pickup was successful
                }
                else
                {
                    // Handle the case where the pickup was not successful
                    Debug.Log("Pickup failed.");
                    return false;
                }
            }
        }
        return false; // No fish to pick up
    }


    public void Sling(Vector3 targetPosition)
    {
        Debug.Log("Sling");
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
