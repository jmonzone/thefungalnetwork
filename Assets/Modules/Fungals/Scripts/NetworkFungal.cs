using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public struct ScoreEventArgs : INetworkSerializable
{
    public Vector3 position;
    public float value;
    public string label;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref value);
        serializer.SerializeValue(ref label);
    }
}

public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private GameReference game;
    [SerializeField] private FungalCollection fungalCollection;

    //todo: make separate prefab map collection
    [SerializeField] private List<GameObject> localPrefabs;
    [SerializeField] private List<NetworkObject> networkPrefabs;

    public FungalController Fungal { get; private set; }
    public FungalData Data => Fungal.Data;
    public Health Health => Fungal.Health;
    public Movement Movement => Fungal.Movement;
    private MovementAnimations Animations => Fungal.Animations;

    private ClientNetworkTransform networkTransform;

    public int Index => index.Value;
    public float Score => score.Value;
    public int Lives => lives.Value;
    public string PlayerName => player.Value.name.ToString();

    // Exposed to all clients, replicated by Netcode
    private NetworkVariable<LobbyPlayerRPCParam> player = new NetworkVariable<LobbyPlayerRPCParam>();
    private NetworkVariable<int> index = new NetworkVariable<int>();

    private NetworkVariable<float> score = new NetworkVariable<float>();
    private NetworkVariable<int> lives = new NetworkVariable<int>(3);
    public NetworkVariable<int> kills = new NetworkVariable<int>(0);
    public NetworkVariable<int> selfDestructs = new NetworkVariable<int>(0);

    public int Kills => kills.Value;
    public int SelfDestructs => selfDestructs.Value;

    public event UnityAction OnLivesChanged;
    public event UnityAction<int, int> OnKill;
    public event UnityAction OnScoreUpdated;
    public event UnityAction<ScoreEventArgs> OnScoreTriggered;
    public event UnityAction OnPlayerUpdated;

    [ServerRpc(RequireOwnership = false)]
    public void InitializeServerRpc(int index, LobbyPlayerRPCParam player)
    {
        //Debug.Log($"InitializeServerRpc {playerName}");
        this.player.Value = player;
        this.index.Value = index;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Fungal = GetComponent<FungalController>();
        Fungal.Id = NetworkObjectId;

        if (IsServer)
        {
            Fungal.OnDeath += OnDeath;
        }

        if (IsOwner)
        {
            Fungal.OnShieldToggled += ToggleShieldRendererServerRpc;
            Fungal.OnTrailToggled += ToggleTrailRenderersServerRpc;
            Movement.OnTypeChanged += Movement_OnTypeChanged;
        }

        Fungal.HandleSpeedModifier = HandleSpeedModiferServerRpc;
        Fungal.HandleSpeedReset = HandleSpeedResetServerRpc;
        Fungal.HandleRespawn = IsOwner ? HandleRespawnServerRpc : null;
        Fungal.HandleDeath = IsOwner ? HandleDeathServerRpc : null;
        Fungal.HandleSpawnObject = (obj, position) =>
        {
            var index = localPrefabs.IndexOf(obj);
            HandleSpawnObjectServerRpc(OwnerClientId, index, position);
        };

        var fishPickup = GetComponent<FishPickup>();
        fishPickup.enabled = IsOwner;

        networkTransform = GetComponent<ClientNetworkTransform>();

        index.OnValueChanged += (old, value) =>
        {
            name = $"{index.Value}: {PlayerName}";
        };

        player.OnValueChanged += (old, value) =>
        {
            //Debug.Log($"NetworkFungal.OnValueChanged {name} {value}");
            name = $"{index.Value}: {value.name}";
            OnPlayerUpdated?.Invoke();
        };

        //Debug.Log($"NetworkFungal {name} {playerName.Value}");
        name = $"{index.Value}: {PlayerName}";

        OnPlayerUpdated?.Invoke();

        score.OnValueChanged += (old, value) =>
        {
            //Debug.Log("score.OnValueChanged");
            OnScoreUpdated?.Invoke();
        };

        if (game.gameMode != GameMode.PARTY)
        {
            lives.OnValueChanged += (old, value) =>
            {
                OnLivesChanged?.Invoke();

                if (IsOwner)
                {
                    Fungal.CanRespawn = value > 0;
                }
            };
        }
            

        if (!string.IsNullOrEmpty(player.Value.lobbyId.ToString()))
        {
            InitializePrefab();
        }
        else
        {
            //Debug.Log($"waiting to initialize");

            player.OnValueChanged += (previousValue, newValue) =>
            {
                InitializePrefab();
            };
        }
    }

    private void InitializePrefab()
    {
        var data = fungalCollection.Fungals[player.Value.fungal];

        Fungal.IsBot = player.Value.isAI;
        Fungal.InitializePrefab(data);
    }

    private void Update()
    {
        if (IsOwner && !Fungal.IsBot && Input.GetKeyUp(KeyCode.L))
        {
            Fungal.Die(true);
        }

        if (IsOwner && Input.GetKeyUp(KeyCode.P))
        {
            AddToScoreServerRpc(new ScoreEventArgs
            {
                value = 200f,
                position = transform.position,
                label = "Debug"
            });
        }
    }

    private void OnDeath(bool selfDestruct)
    {
        if (selfDestruct) selfDestructs.Value++;

        lives.Value--;
        score.Value = Mathf.FloorToInt(score.Value / 2f);
    }

    [ServerRpc]
    private void HandleDeathServerRpc(bool selfDestruct)
    {
        HandleDeathClientRpc(selfDestruct);
    }

    [ClientRpc]
    private void HandleDeathClientRpc(bool selfDestruct)
    {
        Fungal.ApplyDeath(selfDestruct);
    }

    [ServerRpc(RequireOwnership = false)]
    public void HandleRespawnServerRpc()
    {
        HandleRespawnClientRpc();
    }

    [ClientRpc]
    private void HandleRespawnClientRpc()
    {
        Fungal.ApplyRespawnEffects();

        if (IsOwner)
        {
            Health.ReplenishHealth();

            networkTransform.Interpolate = false;
            transform.position = Fungal.SpawnPosition;
            networkTransform.Interpolate = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnKillServerRpc(Vector3 targetPosition, int killVictim)
    {
        kills.Value++;

        AddToScoreServerRpc(new ScoreEventArgs
        {
            value = 250f,
            position = targetPosition,
            label = "KO"
        });

        OnKillClientRpc(Index, killVictim);
    }

    [ClientRpc]
    private void OnKillClientRpc(int killIndex, int killVictim)
    {
        OnKill?.Invoke(killIndex, killVictim);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddToScoreServerRpc(ScoreEventArgs args)
    {
        score.Value += args.value;
        AddToScoreClientRpc(args);
    }

    [ClientRpc]
    private void AddToScoreClientRpc(ScoreEventArgs args)
    {
        OnScoreTriggered?.Invoke(args);
    }

    private void Movement_OnTypeChanged()
    {
        SetAnimationsIsMovingServerRpc(Movement.Type != Movement.MovementType.IDLE);
    }

    [ServerRpc]
    private void SetAnimationsIsMovingServerRpc(bool isMoving)
    {
        SetAnimationsIsMovingClientRpc(isMoving);
    }

    [ClientRpc]
    private void SetAnimationsIsMovingClientRpc(bool isMoving)
    {
        Animations.SetIsMoving(isMoving);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleSpeedModiferServerRpc(float modifer, bool showStunAnimation = false)
    {
        HandleSpeedModifierClientRpc(modifer, showStunAnimation);
    }

    [ClientRpc]
    private void HandleSpeedModifierClientRpc(float modifer, bool showStunAnimation)
    {
        Fungal.ApplySpeedModifier(modifer, showStunAnimation);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleSpeedResetServerRpc(bool showStunAnimation = false)
    {
        HandleSpeedResetClientRpc(showStunAnimation);
    }

    [ClientRpc]
    private void HandleSpeedResetClientRpc(bool showStunAnimation)
    {
        Fungal.ApplySpeedReset(showStunAnimation);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleTrailRenderersServerRpc(bool value)
    {
        ToggleTrailRenderersClientRpc(value);
    }

    [ClientRpc]
    private void ToggleTrailRenderersClientRpc(bool value)
    {
        if (!IsOwner) Fungal.ToggleTrailRenderers(value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleShieldRendererServerRpc(bool value)
    {
        ToggleShieldRenderersClientRpc(value);
    }

    [ClientRpc]
    private void ToggleShieldRenderersClientRpc(bool value)
    {
        if (!IsOwner) Fungal.ToggleShieldRenderers(value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleSpawnObjectServerRpc(ulong clientId, int index, Vector3 spawnPosition)
    {
        var spawnedObj = Instantiate(networkPrefabs[index], spawnPosition, Quaternion.identity);
        spawnedObj.transform.GetChild(0).localScale = Vector3.zero;
        spawnedObj.SpawnWithOwnership(clientId);
        HandleSpawnObjectClientRpc(spawnedObj.NetworkObjectId);
    }

    [ClientRpc]
    private void HandleSpawnObjectClientRpc(ulong networkObjectId)
    {
        Debug.Log("HandleSpawnObjectClientRpc");

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            Fungal.OnObjectSpawned(networkObject.gameObject);
        }
    }
}
