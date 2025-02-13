using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishManager : MonoBehaviour
{
    public event UnityAction OnFishCaught;

    private void Awake()
    {
        var spawners = GetComponentsInChildren<FishSpawner>();
        foreach(var spawner in spawners)
        {
            spawner.OnFishSpawned += Spawner_OnFishSpawned;
        }
    }

    private void Spawner_OnFishSpawned(FishController fish)
    {
        fish.OnCaught += Fish_OnCaught;
    }

    private void Fish_OnCaught()
    {
        OnFishCaught?.Invoke();
    }
}
