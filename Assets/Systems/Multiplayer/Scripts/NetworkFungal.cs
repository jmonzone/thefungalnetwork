using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private FungalData data;
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private GameObject stunAnimation;

    public FungalData Data => data;
    public Health Health { get; private set; }
    public Movement Movement { get; private set; }
    public AudioSource AudioSource { get; private set; }

    //todo: make separate death component
    public bool IsDead { get; private set; }
    public int PlayerIndex { get; private set; }

    private MovementAnimations animations;
    private MaterialFlasher materialFlasher;

    public event UnityAction OnRespawn;
    public event UnityAction OnDeath;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        pufferball.RegisterPlayer(this);

        Health = GetComponent<Health>();
        Movement = GetComponent<Movement>();
        Movement.OnTypeChanged += Movement_OnTypeChanged;

        AudioSource = GetComponent<AudioSource>();

        animations = GetComponent<MovementAnimations>();
        materialFlasher = GetComponent<MaterialFlasher>();

        Health.OnHealthDepleted += Die;
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

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.L)) Die();
    }
    private void Die()
    {
        IsDead = true;

        animations.PlayDeathAnimation();
        Movement.Stop();
        OnDeath?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void InitializeServerRpc(int playerIndex)
    {
        InitializeClientRpc(playerIndex);
    }

    [ClientRpc]
    private void InitializeClientRpc(int playerIndex)
    {
        this.PlayerIndex = playerIndex;
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
        if (IsOwner) transform.position = arena.SpawnPositions[PlayerIndex].position;
        Health.SetHealth(Health.MaxHealth);
        animations.PlaySpawnAnimation();
        OnRespawn?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        TakeDamageClientRpc(damage);
    }

    [ClientRpc]
    private void TakeDamageClientRpc(float damage)
    {
        // Subtract damage from the shield first
        if (Health.CurrentShield > 0)
        {
            float shieldDamage = Mathf.Min(damage, Health.CurrentShield); // Take as much damage as possible from the shield
            Health.SetShield(Health.CurrentShield - shieldDamage); // Reduce shield
            damage -= shieldDamage; // Reduce the remaining damage
        }

        // If there's any remaining damage, subtract it from health
        if (damage > 0)
        {
            
            animations.PlayHitAnimation();
            materialFlasher.FlashColor(Color.white);
            Health.SetHealth(Health.CurrentHealth - damage); // Reduce health with the remaining damage
        }
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
