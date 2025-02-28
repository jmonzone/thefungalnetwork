using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PufferballMinigame : NetworkBehaviour
{
    [SerializeField] private PufferballReference pufferball;

    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference resultsView;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;

    public int CurrentScore { get; private set; }
    public int OpponentScore { get; private set; }

    public event UnityAction OnScoreUpdated;

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
    private void OnPufferballMinigameServerRpc(ulong fungalId)
    {
        OnPufferballMinigameClientRpc(fungalId);
    }

    [ClientRpc]
    private void OnPufferballMinigameClientRpc(ulong fungalId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(fungalId, out var networkObject))
        {
            if (networkObject.IsOwner) CurrentScore++;
            else OpponentScore++;

            OnScoreUpdated?.Invoke();

            if (CurrentScore >= 3 || OpponentScore >= 3)
            {
                headerText.color = networkObject.IsOwner ? loseColor : winColor;
                headerText.text = networkObject.IsOwner ? "Bogged Down" : "Bog Unclogged";

                navigation.Navigate(resultsView);
            }
            else if (networkObject.IsOwner)
            {
                var networkFungal = networkObject.GetComponent<NetworkFungal>();
                networkFungal.RespawnServerRpc();
            }
        }
    }
}
