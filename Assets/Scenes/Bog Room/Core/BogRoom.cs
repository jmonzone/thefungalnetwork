using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

// todo: organize with AutoConnect.cs
public class BogRoom : NetworkBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private ShruneItem defaultShrune;
    [SerializeField] private ItemInventory itemInventory;
    [SerializeField] private MultiplayerArena multiplayerArena;
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private GameClock gameClock;

    [SerializeField] private Light spotlight; // Spotlight component to adjust
    [SerializeField] private float detectionRadius = 5f; // Radius of the overlap circle
    [SerializeField] private float lerpSpeed = 2f; // Speed of the spotlight shape adjustment
    [SerializeField] private int minSpotAngle = 60; // Minimum spotlight angle
    [SerializeField] private int maxSpotAngle = 100; // Maximum spotlight angle

    private float targetSpotAngle;

    [Header("Results")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference resultView;
    [SerializeField] private TextMeshProUGUI resultHeader;
    [SerializeField] private Color winResultColor;
    [SerializeField] private Color loseResultColor;

    private void Awake()
    {
        gameClock.OnCountdownComplete += GameClock_OnCountdownComplete;
        targetSpotAngle = minSpotAngle;
    }

    private void GameClock_OnCountdownComplete()
    {
        if (IsServer) OnLossServerRpc();
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
        //if (playerReference.Movement)
        //{
        //    spotlight.transform.position = playerReference.Movement.transform.position + Vector3.up * 4f;
        //    DetectNearbyFungals();
        //    AdjustSpotlightShape();
        //}
    }

    private void DetectNearbyFungals()
    {
        //Collider[] hits = Physics.OverlapSphere(playerReference.Movement.transform.position, detectionRadius);
        //List<NetworkFungal> currentFungals = new List<NetworkFungal>();

        //foreach (var hit in hits)
        //{
        //    NetworkFungal fungal = hit.GetComponentInParent<NetworkFungal>();
        //    if (fungal && !currentFungals.Contains(fungal)) currentFungals.Add(fungal);
        //}

        //// Update the number of nearby fungals
        //int fungalCount = currentFungals.Count - 1;

        //// Calculate the target spotlight angles based on the number of nearby fungals
        //targetSpotAngle = Mathf.Clamp(minSpotAngle + fungalCount * 30f, minSpotAngle, maxSpotAngle); // Increment by 10 per additional fungal
    }

    private void AdjustSpotlightShape()
    {
        if (spotlight != null)
        {
            spotlight.innerSpotAngle = Mathf.Lerp(spotlight.innerSpotAngle, targetSpotAngle, lerpSpeed * Time.deltaTime);
            spotlight.spotAngle = spotlight.innerSpotAngle + 10;
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
        //playerReference.OnDeath += OnLossServerRpc;
    }

    private void OnDisable()
    {
        multiplayerArena.OnAllMushroomsCollected -= MultiplayerArena_OnAllMushroomsCollected;
        //playerReference.OnDeath -= OnLossServerRpc;
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
        //playerReference.Movement.Stop();
        //playerReference.Movement.enabled = false;
        StartCoroutine(WaitToShowResults());
    }

    private IEnumerator WaitToShowResults()
    {
        yield return new WaitForSeconds(2f);
        navigation.Navigate(resultView);
    }
}
