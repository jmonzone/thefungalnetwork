using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

// todo: organize with AutoConnect.cs
public class BogRoom : NetworkBehaviour
{
    [SerializeField] private Controller playerReference;
    [SerializeField] private ShruneItem defaultShrune;
    [SerializeField] private ItemInventory itemInventory;
    [SerializeField] private MultiplayerArena multiplayerArena;
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private Transform lightTransform;
    [SerializeField] private GameClock gameClock;

    [Header("Results")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference resultView;
    [SerializeField] private TextMeshProUGUI resultHeader;
    [SerializeField] private Color winResultColor;
    [SerializeField] private Color loseResultColor;

    private void Awake()
    {
        gameClock.OnCountdownComplete += GameClock_OnCountdownComplete;
    }

    private void GameClock_OnCountdownComplete()
    {
        OnLossServerRpc();
    }

    private void Start()
    {
        if (itemInventory.GetItemCount(defaultShrune) == 0)
        {
            itemInventory.AddToInventory(defaultShrune, 1);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            StartCoroutine(WaitForLobby());
        }
    }

    private void Update()
    {
        if (playerReference.Movement)
        {
            lightTransform.position = playerReference.Movement.transform.position + Vector3.up * 4f;
        }
    }

    private IEnumerator WaitForLobby()
    {
        yield return new WaitUntil(() => multiplayer.JoinedLobby != null);
        yield return new WaitUntil(() => multiplayerArena.Players.Count == multiplayer.JoinedLobby.Players.Count);
        StartGameServerRpc();
        yield return new WaitForSeconds(15f);
        var randomIndex = Random.Range(0, multiplayerArena.Players.Count);
        var randomPlayer = multiplayerArena.Players[randomIndex];
        var networkFungal = randomPlayer.GetComponent<NetworkFungal>();
        //networkFungal.SetAsMinionServerRpc();
        AssignMinionClientRpc(networkFungal.OwnerClientId);
    }

    [ClientRpc]
    private void AssignMinionClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) multiplayerArena.AssignMinion(clientId);
    }

    private void OnEnable()
    {
        Debug.Log($"OnEnable called on client: {IsClient}, server: {IsServer}");
        multiplayerArena.OnAllMushroomsCollected += MultiplayerArena_OnAllMushroomsCollected;
        playerReference.OnDeath += OnLossServerRpc;
    }

    private void OnDisable()
    {
        multiplayerArena.OnAllMushroomsCollected -= MultiplayerArena_OnAllMushroomsCollected;
        playerReference.OnDeath -= OnLossServerRpc;
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnLossServerRpc()
    {
        OnLossClientRpc();
    }

    [ClientRpc]
    private void OnLossClientRpc()
    {
        resultHeader.text = "Bogged Down?";
        resultHeader.color = loseResultColor;
        ShowResults();
    }


    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        multiplayerArena.InvokeIntroComplete();
    }

    private void MultiplayerArena_OnAllMushroomsCollected()
    {
        resultHeader.text = "Bog Unclogged!";
        resultHeader.color = winResultColor;
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
