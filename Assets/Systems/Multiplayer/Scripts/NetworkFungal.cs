using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private GameObject stunAnimation;

    private Health health;
    private Movement movement;
    private MaterialFlasher materialFlasher;

    public int playerIndex { get; private set; }

    public event UnityAction OnHealthDepleted;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        pufferball.RegisterPlayer(this);

        health = GetComponent<Health>();
        movement = GetComponent<Movement>();
        materialFlasher = GetComponent<MaterialFlasher>();

        health.OnHealthDepleted += Health_OnHealthDepleted;
    }

    private void Health_OnHealthDepleted()
    {
        movement.Stop();
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
        RespawnClientRpc();
    }

    [ClientRpc]
    private void RespawnClientRpc()
    {
        if (IsOwner) transform.position = arena.SpawnPositions[playerIndex].position;
        health.SetHealth(health.MaxHealth);
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
        if (health.CurrentShield > 0)
        {
            float shieldDamage = Mathf.Min(damage, health.CurrentShield); // Take as much damage as possible from the shield
            health.SetShield(health.CurrentShield - shieldDamage); // Reduce shield
            damage -= shieldDamage; // Reduce the remaining damage
        }

        // If there's any remaining damage, subtract it from health
        if (damage > 0)
        {
            materialFlasher.FlashColor(Color.white);
            health.SetHealth(health.CurrentHealth - damage); // Reduce health with the remaining damage
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
        health.SetShield(health.CurrentShield + value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ModifySpeedServerRpc(float modifer, float duration)
    {
        ModifySpeedClientRpc(modifer, duration);
    }

    [ClientRpc]
    private void ModifySpeedClientRpc(float modifer, float duration)
    {
        stunAnimation.SetActive(true);

        if (IsOwner)
        {
            movement.SetSpeedModifier(modifer);
            Invoke(nameof(ResetSpeedClientRpc), duration);
        }
    }

    [ClientRpc]
    private void ResetSpeedClientRpc()
    {
        stunAnimation.SetActive(false);
        movement.ResetSpeedModifier();
    }
}
