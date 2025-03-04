using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishPickup : MonoBehaviour
{
    public Fish Fish { get; private set; }

    public event UnityAction OnFishChanged;
    public event UnityAction OnFishReleased;

    private void Update()
    {
        if (!Fish) DetectPufferfishHit();

    }

    public void Sling(Vector3 targetPosition)
    {
        Debug.Log("Sling");
        if (Fish)
        {
            Fish.Throw(targetPosition);
            OnFishRemoved();
        }
    }

    private void OnFishRemoved()
    {
        var networkPufferfish = Fish.GetComponent<Pufferfish>(); // Ensure it's the correct one
        if (networkPufferfish != null)
        {
            networkPufferfish.OnMaxTemperReached -= NetworkPufferfish_OnMaxTemperReached;
        }

        Fish = null;
        OnFishChanged?.Invoke();
    }

    private bool DetectPufferfishHit()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f); // Small detection radius

        foreach (Collider hit in hits)
        {
            var fish = hit.GetComponentInParent<Fish>();
            if (fish != null && fish.CanPickUp)
            {
                var networkPufferfish = fish.GetComponent<Pufferfish>(); // Ensure it's the correct one
                if (networkPufferfish != null)
                {
                    networkPufferfish.OnMaxTemperReached += NetworkPufferfish_OnMaxTemperReached;
                }


                Fish = fish;
                Fish.PickUp();
                OnFishChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    private void NetworkPufferfish_OnMaxTemperReached()
    {
        OnFishRemoved();
        OnFishReleased?.Invoke();
    }
}
