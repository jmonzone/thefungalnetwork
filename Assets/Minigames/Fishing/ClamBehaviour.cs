using UnityEngine;
using UnityEngine.Events;

public class ClamBehaviour : MonoBehaviour
{
    private FishController fish;
    private bool isOpen = false;

    private void Awake()
    {
        fish = GetComponent<FishController>();
        fish.OnHit += OnHit;
    }

    private void OnHit()
    {
        if (!isOpen) isOpen = true;
        else fish.Catch();
    }
}
