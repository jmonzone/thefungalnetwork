using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private MultiplayerArena arena;

    private Health health;
    private Movement movement;
    public int playerIndex { get; private set; }

    public event UnityAction OnHealthDepleted;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        pufferball.RegisterPlayer(this);

        health = GetComponent<Health>();
        movement = GetComponent<Movement>();

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
        transform.position = arena.SpawnPositions[playerIndex].position;
        RespawnClientRpc();
    }

    [ClientRpc]
    private void RespawnClientRpc()
    {
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
        health.SetHealth(health.CurrentHealth - damage);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ModifySpeedServerRpc(float modifer, float duration)
    {
        ModifySpeedClientRpc(modifer, duration);
    }

    [ClientRpc]
    private void ModifySpeedClientRpc(float modifer, float duration)
    {
        if (IsOwner)
        {
            movement.SetSpeedModifier(modifer);
            Invoke(nameof(ResetSpeed), duration);
        }
    }

    private void ResetSpeed()
    {
        movement.ResetSpeedModifier();
    }
}
