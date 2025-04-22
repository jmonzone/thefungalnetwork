using UnityEngine;
using UnityEngine.Events;

public class FishPickup : MonoBehaviour
{
    private FungalController fungal;
    [SerializeField] private FishController fish;

    public FishController Fish => fish;

    public event UnityAction OnFishChanged;
    public event UnityAction OnFishReleased;

    private void Awake()
    {
        fungal = GetComponent<FungalController>();
        fungal.OnDeath += _ => ReleaseFish();
    }

    private void Update()
    {
        if (!fungal.IsDead) TryPickUpFish();
    }

    private void TryPickUpFish()
    {
        //Debug.Log("Picking up fish");
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f); // Small detection radius

        foreach (Collider hit in hits)
        {
            var fish = hit.GetComponentInParent<FishController>();
            if (fish != null && this.fish != fish && !fish.IsPickedUp)
            {
                ReleaseFish();
                this.fish = fish;
                fish.PickUp(fungal);
                OnFishChanged?.Invoke();
                break;
            }
        }
    }

    private void ReleaseFish()
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

        fish = null;
        OnFishChanged?.Invoke();
    }
}
