using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishPickup : MonoBehaviour
{
    public Fish Fish { get; private set; }

    public event UnityAction OnFishChanged;

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
        Fish = null;
    }

    private bool DetectPufferfishHit()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f); // Small detection radius

        foreach (Collider hit in hits)
        {
            Fish = hit.GetComponentInParent<Fish>();
            if (Fish != null && Fish.CanPickUp)
            {
                Fish.PickUp();
                OnFishChanged?.Invoke();
                return true;
            }
        }
        return false;
    }
}
