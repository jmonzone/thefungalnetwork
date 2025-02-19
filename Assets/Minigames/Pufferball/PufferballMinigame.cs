using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PufferballMinigame : NetworkBehaviour
{
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference resultsView;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer) pufferball.OnPlayerDefeated += OnPufferballMinigameServerRpc;
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer) pufferball.OnPlayerDefeated -= OnPufferballMinigameServerRpc;
    }

    [ServerRpc]
    private void OnPufferballMinigameServerRpc(ulong playerId)
    {
        OnPufferballMinigameClientRpc(playerId);
    }

    [ClientRpc]
    private void OnPufferballMinigameClientRpc(ulong playerId)
    {
        var isOwner = playerId == NetworkManager.Singleton.LocalClientId;
        headerText.color = isOwner ? loseColor : winColor;
        headerText.text = isOwner ? "Bogged Down" : "Bog Unclogged";

        navigation.Navigate(resultsView);
    }
}
