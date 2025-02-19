using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private FungalInventory fungalInventory;

    private Health health;

    public NetworkVariable<float> Health = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public event UnityAction OnHealthDepleted;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        pufferball.RegisterPlayer(this);

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
    public void TakeDamageServerRpc(float damage)
    {
        Health.Value = Mathf.Clamp(Health.Value - damage, 0, health.MaxHealth);
        if (Health.Value == 0) OnHealthDepleted?.Invoke();
    }
}
