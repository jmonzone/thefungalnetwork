using System.Collections;
using Unity.Collections;
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
    [SerializeField] private FungalData data;
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private GameObject stunAnimation;
    [SerializeField] private float respawnDuration = 5f;
    [SerializeField] private FungalCollection fungalCollection;

    [SerializeField] private float baseSpeed = 3f;
    public float BaseSpeed => 3f;

    public FungalData Data => data;
    public Health Health { get; private set; }
    public Movement Movement { get; private set; }

    public float RemainingRespawnTime { get; private set; }

    //todo: make separate death component
    public bool IsDead { get; private set; }

    private Vector3 spawnPosition;

    private ClientNetworkTransform networkTransform;
    private MovementAnimations animations;
    private MaterialFlasher materialFlasher;
    private FungalAI fungalAI;

    public int Index => index.Value;
    public float Score => score.Value;
    public int Lives => lives.Value;
    public string PlayerName => playerName.Value.ToString();

    // Exposed to all clients, replicated by Netcode
    private NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>();
    private NetworkVariable<int> index = new NetworkVariable<int>();
    private NetworkVariable<int> fungalIndex = new NetworkVariable<int>(-1);
    public NetworkVariable<bool> isAI = new NetworkVariable<bool>(false);

    private NetworkVariable<float> score = new NetworkVariable<float>();
    private NetworkVariable<int> lives = new NetworkVariable<int>(3);
    public NetworkVariable<int> kills = new NetworkVariable<int>(0);
    public NetworkVariable<int> selfDestructs = new NetworkVariable<int>(0);

    public int Kills => kills.Value;
    public int SelfDestructs => selfDestructs.Value;

    public event UnityAction OnRespawnStart;
    public event UnityAction OnRespawnComplete;
    public event UnityAction<bool> OnDeath;
    public event UnityAction OnLivesChanged;
    public event UnityAction<int, int> OnKill;
    public event UnityAction OnScoreUpdated;
    public event UnityAction<ScoreEventArgs> OnScoreTriggered;
    public event UnityAction OnPlayerUpdated;

    [ServerRpc(RequireOwnership = false)]
    public void InitializeServerRpc(FixedString64Bytes playerName, int index, int fungalIndex, bool isAI)
    {
        //Debug.Log($"InitializeServerRpc {playerName}");
        this.playerName.Value = playerName;
        this.fungalIndex.Value = fungalIndex;
        this.index.Value = index;
        this.isAI.Value = isAI;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Health = GetComponent<Health>();
        Movement = GetComponent<Movement>();
        Movement.SetSpeed(baseSpeed);

        Movement.OnTypeChanged += Movement_OnTypeChanged;

        networkTransform = GetComponent<ClientNetworkTransform>();
        fungalAI = GetComponent<FungalAI>();

        spawnPosition = transform.position;

        Health.OnDamaged += Health_OnDamaged;

        index.OnValueChanged += (old, value) =>
        {
            name = $"{index.Value}: {playerName.Value}";
        };

        playerName.OnValueChanged += (old, value) =>
        {
            //Debug.Log($"NetworkFungal.OnValueChanged {name} {value}");
            name = $"{index.Value}: {playerName.Value}";
            OnPlayerUpdated?.Invoke();
        };

        //Debug.Log($"NetworkFungal {name} {playerName.Value}");
        name = $"{index.Value}: {playerName.Value}";
        OnPlayerUpdated?.Invoke();

        isAI.OnValueChanged += (old, value) =>
        {
            if (value)
            {
                ToggleAI(true);
            }
        };

        score.OnValueChanged += (old, value) =>
        {
            //Debug.Log("score.OnValueChanged");
            OnScoreUpdated?.Invoke();
        };

        lives.OnValueChanged += (old, value) =>
        {
            OnLivesChanged?.Invoke();

            if (IsOwner || (IsServer && isAI.Value))
            {
                if (pufferball.gameMode == GameMode.ELIMINATION && value > 0 || pufferball.gameMode == GameMode.PARTY)
                {
                    StartCoroutine(RespawnRoutine());
                }
            }
        };

        //Debug.Log($"NetworkFungal.OnNetworkSpawn {fungalIndex.Value}");
        if (fungalIndex.Value != -1)
        {
            InitializePrefab();
        }
        else
        {
            fungalIndex.OnValueChanged += (previousValue, newValue) =>
            {
                InitializePrefab();
            };
        }
    }

    private void ToggleAI(bool value)
    {
        if (IsOwner && isAI.Value)
        {
            fungalAI.enabled = value;
        }
    }

    private void InitializePrefab()
    {
        data = fungalCollection.Fungals[fungalIndex.Value];
        var model = Instantiate(data.Prefab, transform);
        animations = model.AddComponent<MovementAnimations>();
        materialFlasher = model.AddComponent<MaterialFlasher>();
        materialFlasher.flashDuration = 0.5f;
    }

    private void Health_OnDamaged(DamageEventArgs args)
    {
        if (args.lethal)
        {
            if (IsServer) DieServerRpc(args.SelfInflicted);
        }
        else
        {
            animations.PlayHitAnimation();
            materialFlasher.FlashColor(Color.red);
        }
    }

    private void Update()
    {
        if (IsOwner && !isAI.Value && Input.GetKeyUp(KeyCode.L))
        {
            DieServerRpc(true);
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

    [ServerRpc(RequireOwnership = false)]
    private void DieServerRpc(bool selfDestruct)
    {
        //Debug.Log("DieClientRpc");
        if (selfDestruct) selfDestructs.Value++;

        lives.Value--;
        score.Value = Mathf.FloorToInt(score.Value / 2f);
        DieClientRpc(selfDestruct);
    }

    [ClientRpc]
    private void DieClientRpc(bool selfDestruct)
    {
        ToggleAI(false);
        //Debug.Log("DieClientRpc");
        IsDead = true;
        animations.PlayDeathAnimation();
        materialFlasher.FlashColor(Color.red);

        Movement.Stop();
        OnDeath?.Invoke(selfDestruct);
    }

    private IEnumerator RespawnRoutine()
    {
        OnRespawnStart?.Invoke();
        RemainingRespawnTime = respawnDuration;

        while (RemainingRespawnTime > 0f)
        {
            yield return null; // Wait for next frame
            RemainingRespawnTime -= Time.deltaTime;
        }

        RemainingRespawnTime = 0f;

        RespawnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnServerRpc()
    {
        RespawnClientRpc();
    }

    [ClientRpc]
    private void RespawnClientRpc()
    {
        IsDead = false;

        if (IsServer)
        {
            Health.Replenish();
        }

        if (IsOwner)
        {
            networkTransform.Interpolate = false;
            transform.position = spawnPosition;
            networkTransform.Interpolate = true;

            ToggleAI(true);
        }

        animations.PlaySpawnAnimation();
        materialFlasher.FlashColor(Color.white);
        OnRespawnComplete?.Invoke();
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
        if (IsOwner) SyncMovementTypeServerRpc((int)Movement.Type);
    }

    
    [ServerRpc]
    private void SyncMovementTypeServerRpc(int type)
    {
        SyncMovementTypeClientRpc(type);
    }

    [ClientRpc]
    private void SyncMovementTypeClientRpc(int type)
    {
        if (!IsOwner) Movement.SetType((Movement.MovementType)type);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddShieldServerRpc(float value)
    {
        AddShieldClientRpc(value);
    }

    [ClientRpc]
    private void AddShieldClientRpc(float value)
    {
        Health.SetShield(Health.CurrentShield + value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ModifySpeedServerRpc(float modifer, float duration, bool showStunAnimation = false)
    {
        ModifySpeedClientRpc(modifer, showStunAnimation);
        StartCoroutine(ResetSpeedClientRpcRoutine(duration, showStunAnimation));
    }

    private IEnumerator ResetSpeedClientRpcRoutine(float duration, bool showStunAnimatio)
    {
        yield return new WaitForSeconds(duration);
        ResetSpeedClientRpc(showStunAnimatio);
    }

    [ClientRpc]
    private void ModifySpeedClientRpc(float modifer, bool showStunAnimation)
    {
        if (showStunAnimation) stunAnimation.SetActive(true);

        if (IsOwner)
        {
            Movement.SetSpeedModifier(modifer);
        }
    }

    [ClientRpc]
    private void ResetSpeedClientRpc(bool showStunAnimation)
    {
        if (showStunAnimation) stunAnimation.SetActive(false);
        Movement.ResetSpeedModifier();
    }
}
