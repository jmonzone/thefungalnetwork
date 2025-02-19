using Unity.Netcode;
using UnityEngine;

public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FungalInventory fungalInventory;

    private Health health;

    public NetworkVariable<float> Health = new NetworkVariable<float>(
    0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        arena.RegisterPlayer(transform);

        var fungalController = GetComponent<FungalController>();
        fungalController.InitializeAnimations();

        health = GetComponent<Health>();


        if (IsServer)
        {
            Health.Value = health.CurrentHealth;
        }

        Health.OnValueChanged += (oldValue, newValue) => health.SetHealth(newValue);
    }

    public override void OnNetworkDespawn()
    {
        Health.OnValueChanged -= (oldValue, newValue) => health.SetHealth(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float health)
    {
        Debug.Log("TakeDamageServerRpc");
        Health.Value -= health;
    }
}
