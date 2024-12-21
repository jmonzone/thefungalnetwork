using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class BossRoom : NetworkBehaviour
{
    [SerializeField] private Controller playerReference;
    [SerializeField] private ShruneItem defaultShrune;
    [SerializeField] private ItemInventory itemInventory;
    [SerializeField] private MultiplayerArena multiplayerArena;

    [Header("Results")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference resultView;
    [SerializeField] private TextMeshProUGUI resultHeader;
    [SerializeField] private Color winResultColor;
    [SerializeField] private Color loseResultColor;

    private void Start()
    {
        if (itemInventory.GetItemCount(defaultShrune) == 0)
        {
            itemInventory.AddToInventory(defaultShrune, 1);
        }
    }

    private void OnEnable()
    {
        Debug.Log($"OnEnable called on client: {IsClient}, server: {IsServer}");
        multiplayerArena.OnAllMushroomsCollected += MultiplayerArena_OnAllMushroomsCollected;
        multiplayerArena.OnAllPlayersDead += MultiplayerArena_OnAllPlayersDead;
        playerReference.OnDeath += TriggerDeathEventServerRpc;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TriggerDeathEventServerRpc()
    {
        OnPlayerDeathClientRpc();
    }

    [ClientRpc]
    private void OnPlayerDeathClientRpc()
    {
        Debug.Log("OnPlayerDeathClientRpc invoked on client.");
        multiplayerArena.IncrementDeadPlayerCount();
    }

    private void MultiplayerArena_OnAllMushroomsCollected()
    {
        resultHeader.text = "Bog Unclogged!";
        resultHeader.color = winResultColor;
        ShowResults();
    }

    private void OnDisable()
    {
        multiplayerArena.OnAllMushroomsCollected -= MultiplayerArena_OnAllMushroomsCollected;
        multiplayerArena.OnAllPlayersDead -= MultiplayerArena_OnAllPlayersDead;
        playerReference.OnDeath -= OnPlayerDeathClientRpc;
    }


    private void MultiplayerArena_OnAllPlayersDead()
    {
        resultHeader.text = "Bogged Down?";
        resultHeader.color = loseResultColor;
        ShowResults();
    }

    private void ShowResults()
    {
        playerReference.Movement.Stop();
        playerReference.Movement.enabled = false;
        StartCoroutine(WaitToShowResults());
    }

    private IEnumerator WaitToShowResults()
    {
        yield return new WaitForSeconds(2f);
        navigation.Navigate(resultView);
    }
}
