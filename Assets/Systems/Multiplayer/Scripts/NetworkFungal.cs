using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private FungalData data;
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private GameObject stunAnimation;
    [SerializeField] private float respawnDuration = 5f;

    public FungalData Data => data;
    public Health Health { get; private set; }
    public Movement Movement { get; private set; }
    public AudioSource AudioSource { get; private set; }

    public float RemainingRespawnTime { get; private set; }

    //todo: make separate death component
    public bool IsDead { get; private set; }

    private Vector3 spawnPosition;

    private MovementAnimations animations;
    private MaterialFlasher materialFlasher;

    public event UnityAction OnRespawnStart;
    public event UnityAction OnRespawnComplete;

    public event UnityAction OnDeath;

    // Exposed to all clients, replicated by Netcode
    private NetworkVariable<int> index = new NetworkVariable<int>();
    public NetworkVariable<float> Score = new NetworkVariable<float>();

    public int Index => index.Value;

    public void Initialize(int index)
    {
        if (IsServer)
        {
            this.index.Value = index;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Health = GetComponent<Health>();
        Movement = GetComponent<Movement>();
        Movement.OnTypeChanged += Movement_OnTypeChanged;

        AudioSource = GetComponent<AudioSource>();

        animations = GetComponent<MovementAnimations>();
        materialFlasher = GetComponent<MaterialFlasher>();

        spawnPosition = transform.position;

        if (IsOwner)
        {
            Health.OnDamaged += Health_OnDamaged;
            Health.OnHealthDepleted += Die;
        }
    }

    private void Health_OnDamaged()
    {
        animations.PlayHitAnimation();
        materialFlasher.FlashColor(Color.white);
    }

    private void Update()
    {
        if (IsOwner && Input.GetKeyUp(KeyCode.L))
        {
            Die();
        }
    }

    private void Die()
    {
        DieServerRpc();
    }

    [ServerRpc]
    private void DieServerRpc()
    {
        DieClientRpc();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        IsDead = true;
        if (IsOwner) StartCoroutine(RespawnRoutine());
        animations.PlayDeathAnimation();
        Movement.Stop();
        OnDeath?.Invoke();
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

    [ServerRpc]
    public void RespawnServerRpc()
    {
        RespawnClientRpc();
    }

    [ClientRpc]
    private void RespawnClientRpc()
    {
        IsDead = false;
        if (IsOwner)
        {
            transform.position = spawnPosition;
            Health.Replenish();
        }
        animations.PlaySpawnAnimation();
        OnRespawnComplete?.Invoke();
    }

    private void Movement_OnTypeChanged()
    {
        if (IsOwner) SyncMovementTypeServerRpc((int)Movement.Type);
    }

    public void PlayAudioClip(AudioClip audioClip, float pitch)
    {
        AudioSource.clip = audioClip;
        //AudioSource.pitch = pitch;
        AudioSource.Play();
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
    public void ModifySpeedServerRpc(float modifer, float duration)
    {
        ModifySpeedClientRpc(modifer);
        Invoke(nameof(ResetSpeedClientRpc), duration);
    }

    [ClientRpc]
    private void ModifySpeedClientRpc(float modifer)
    {
        stunAnimation.SetActive(true);

        if (IsOwner)
        {
            Movement.SetSpeedModifier(modifer);
        }
    }

    [ClientRpc]
    private void ResetSpeedClientRpc()
    {
        stunAnimation.SetActive(false);
        Movement.ResetSpeedModifier();
    }
}
