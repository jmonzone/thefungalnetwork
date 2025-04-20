using UnityEngine;
using UnityEngine.Events;

public class FishPickup : MonoBehaviour
{
    private FungalController fungal;

    public FishController Fish { get; private set; }

    public event UnityAction OnFishChanged;
    public event UnityAction OnFishReleased;

    private void Awake()
    {
        fungal = GetComponent<FungalController>();
        fungal.OnDeath += _ => Fungal_OnDeath();
    }

    private void Update()
    {
        if (!Fish && !fungal.IsDead) TryPickUpFish();
    }

    private void TryPickUpFish()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f); // Small detection radius

        foreach (Collider hit in hits)
        {
            var fish = hit.GetComponentInParent<FishController>();
            if (fish != null && !fish.IsPickedUp)
            {
                Fish = fish;
                fish.PickUp(fungal);
                OnFishChanged?.Invoke();
                break;
            }
        }
    }

    private void Fungal_OnDeath()
    {
        if (Fish)
        {
            Fish.Respawn();
            RemoveFish();
        }
    }

    public void Sling(Vector3 targetPosition)
    {
        if (Fish)
        {
            Fish.ThrowFish.Throw(targetPosition);
            RemoveFish();
        }
    }

    private void RemoveFish()
    {
        var networkPufferfish = Fish.GetComponent<NetworkFireFish>(); // Ensure it's the correct one
        if (networkPufferfish != null)
        {
            //networkPufferfish.OnMaxTemperReached -= NetworkPufferfish_OnMaxTemperReached;
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
