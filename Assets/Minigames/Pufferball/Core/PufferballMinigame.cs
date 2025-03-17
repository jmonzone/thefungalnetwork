using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

[Obsolete]
public class PufferballMinigame : NetworkBehaviour
{
    //[SerializeField] private PufferballReference pufferball;

    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn();

    //    pufferball.OnPlayerDefeated += OnPlayerDefeated;
    //}


    //public override void OnNetworkDespawn()
    //{
    //    pufferball.OnPlayerDefeated -= OnPlayerDefeated;
    //}

    //private void OnPlayerDefeated(NetworkFungal fungal, int source)
    //{
    //    Debug.Log("OnPlayerDefeated");
    //    pufferball.OnPlayerDeath(fungal, source);
    //}
}
