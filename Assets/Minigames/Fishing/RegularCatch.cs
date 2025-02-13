using UnityEngine;

public class RegularCatch : MonoBehaviour
{
    private FishController fish;

    private void Awake()
    {
        fish = GetComponent<FishController>();
        fish.OnHit += OnHit;
    }

    private void OnHit()
    {
        fish.Catch();
    }
}
