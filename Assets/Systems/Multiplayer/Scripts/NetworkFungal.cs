using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private GameObject stunAnimation;

    public Health Health { get; private set; }
    public Movement Movement { get; private set; }
    private MovementAnimations animations;
    private MaterialFlasher materialFlasher;

    public int playerIndex { get; private set; }

    public event UnityAction OnHealthDepleted;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        pufferball.RegisterPlayer(this);

        Health = GetComponent<Health>();
        Movement = GetComponent<Movement>();
        Movement.OnTypeChanged += Movement_OnTypeChanged;
        animations = GetComponent<MovementAnimations>();
        materialFlasher = GetComponent<MaterialFlasher>();

        Health.OnHealthDepleted += Health_OnHealthDepleted;
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

    private void Health_OnHealthDepleted()
    {
        animations.PlayDeathAnimation();
        Movement.Stop();
        OnHealthDepleted?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void InitializeServerRpc(int playerIndex)
    {
        InitializeClientRpc(playerIndex);
    }

    [ClientRpc]
    private void InitializeClientRpc(int playerIndex)
    {
        this.playerIndex = playerIndex;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnServerRpc()
    {
        Invoke(nameof(RespawnClientRpc), 2f);
    }

    [ClientRpc]
    private void RespawnClientRpc()
    {
        if (IsOwner) transform.position = arena.SpawnPositions[playerIndex].position;
        Health.SetHealth(Health.MaxHealth);
        animations.PlaySpawnAnimation();
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
