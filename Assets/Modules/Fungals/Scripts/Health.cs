using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public struct DamageEventArgs : INetworkSerializable
{
    public bool lethal;
    public int source;
    public int target;
    public bool SelfInflicted => source == target;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref lethal);
        serializer.SerializeValue(ref source);
        serializer.SerializeValue(ref target);
    }
}

public class Health : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private NetworkVariable<float> currentHealth = new NetworkVariable<float>();
    private float currentShield;

    public float CurrentHealth => currentHealth.Value;
    public float CurrentShield => currentShield;
    public float MaxHealth => maxHealth;

    public event UnityAction<DamageEventArgs> OnDamaged;
    public event UnityAction OnHealthChanged;
    public event UnityAction OnHealthDepleted;
    public event UnityAction OnShieldChanged;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        //Debug.Log($"HealthOnNetworkSpawn {IsOwner}");
        if (IsServer) Replenish();

        currentHealth.OnValueChanged += (previousValue, newValue) =>
        {
            OnHealthChanged?.Invoke();

            if (previousValue > 0 && currentHealth.Value <= 0)
            {
                OnHealthDepleted?.Invoke();
            }
        };
    }

    public void Replenish()
    {
        OnReplenishServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnReplenishServerRpc()
    {
        currentHealth.Value = maxHealth;
    }

    public void Damage(float damage, ulong sourceId)
    {
        //Debug.Log($"Damage {damage} {sourceId}");
        OnDamageServerRpc(damage, sourceId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnDamageServerRpc(float damage, ulong sourceId)
    {
        //Debug.Log($"OnDamageServerRpc {damage} {sourceId} {currentHealth.Value}");

        if (currentHealth.Value == 0) return;

        currentHealth.Value = Mathf.Clamp(currentHealth.Value - damage, 0, maxHealth);


        var knockout = currentHealth.Value <= 0;

        var fungal = GetComponentInParent<NetworkFungal>();

        var args = new DamageEventArgs()
        {
            lethal = knockout,
            target = fungal.Index,
            source = fungal.Index,
        };

        if (sourceId != NetworkObjectId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(sourceId, out var networkObject))
            {
                var networkFungal = networkObject.GetComponent<NetworkFungal>();

                args.source = networkFungal.Index;

                if (knockout) networkFungal.OnKillServerRpc(transform.position, fungal.Index);
                else networkFungal.AddToScoreServerRpc(new ScoreEventArgs
                {
                    value = 35f,
                    position = transform.position,
                    label = "Hit"
                });
            }
        }

        

        OnDamageClientRpc(args);
    }

    [ClientRpc]
    private void OnDamageClientRpc(DamageEventArgs args)
    {
        OnDamaged?.Invoke(args);
    }

    public void SetShield(float shield)
    {
        currentShield = Mathf.Max(0, shield);
        OnShieldChanged?.Invoke();
    }
}
