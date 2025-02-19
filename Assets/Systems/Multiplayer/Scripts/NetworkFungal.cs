using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private FungalInventory fungalInventory;

    private Health health;
    private Movement movement;

    public event UnityAction OnHealthDepleted;

    private void Awake()
    {
        Debug.Log("Awake");
        health = GetComponent<Health>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        pufferball.RegisterPlayer(this);

        health = GetComponent<Health>();
        movement = GetComponent<Movement>();

        Debug.Log("OnNetworkSpawn");
        health.OnHealthDepleted += Health_OnHealthDepleted;
    }

    private void Health_OnHealthDepleted()
    {
        movement.Stop();
        OnHealthDepleted?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        TakeDamageClientRpc(damage);
    }

    [ClientRpc]
    private void TakeDamageClientRpc(float damage)
    {
        Debug.Log("TakeDamageClientRpc");
        health.SetHealth(health.CurrentHealth - damage);
    }
}
