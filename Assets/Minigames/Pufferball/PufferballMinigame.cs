using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PufferballMinigame : NetworkBehaviour
{
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference resultsView;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        pufferball.OnPlayerDefeated += Pufferball_OnPlayerDefeated;
    }

    private void Pufferball_OnPlayerDefeated()
    {
        navigation.Navigate(resultsView);
    }
}
