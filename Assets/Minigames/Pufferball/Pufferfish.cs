using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Pufferfish : NetworkBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float minExplosionRadius = 3f;
    [SerializeField] private float maxExplosionRadius = 3f;

    private Movement movement;
    private Fish fish;
    private PufferfishExplosion pufferfishExplosion;
    private PufferfishTemper pufferfishTemper;

    public event UnityAction OnMaxTemperReached;

    public NetworkVariable<float> Temper = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        movement = GetComponent<Movement>();

        fish = GetComponent<Fish>();
        fish.OnPickup += StartTemperServerRpc;

        var pufferfishSling = GetComponent<PufferfishSling>();
        pufferfishSling.OnSlingComplete += HandleSlingComplete;

        pufferfishExplosion = GetComponent<PufferfishExplosion>();
        pufferfishExplosion.OnExplodeComplete += HandleExplodeComplete;

        pufferfishTemper = GetComponent<PufferfishTemper>();
        pufferfishTemper.OnMaxTemperReached += HandleMaxTemperReached;
    }

    private void Update()
    {
        if (IsServer)
        {
            Temper.Value = pufferfishTemper.Temper;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Temper.OnValueChanged += HandleTemperChanged;
    }

    public override void OnNetworkDespawn()
    {
        Temper.OnValueChanged -= HandleTemperChanged;
    }

    private void HandleTemperChanged(float oldValue, float newValue)
    {
        pufferfishTemper.SetTemper(newValue);
    }

    private void HandleSlingComplete()
    {
        if (IsOwner) Explode();
    }

    private void HandleExplodeComplete()
    {
        if (IsOwner)
        {
            fish.InvokeReturnToRadialMovement();
            StopTemperServerRpc();
        }
    }

    private void HandleMaxTemperReached()
    {
        if (IsOwner)
        {
            Explode();
            OnMaxTemperReached?.Invoke();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartTemperServerRpc()
    {
        pufferfishTemper.StartTemper();
    }

    [ServerRpc(RequireOwnership = false)]
    public void StopTemperServerRpc()
    {
        pufferfishTemper.StopTimer();
    }

    private void Explode()
    {
        movement.Stop();
        float radius = pufferfishTemper.Temper * (maxExplosionRadius - minExplosionRadius) + minExplosionRadius;
        pufferfishExplosion.DealExplosionDamage(radius);
        ExplodeServerRpc(radius);
    }


    [ServerRpc]
    public void ExplodeServerRpc(float radius)
    {
        ExplodeClientRpc(radius);
    }

    [ClientRpc]
    private void ExplodeClientRpc(float radius)
    {
        pufferfishExplosion.StartExplosionAnimation(radius);
    }
}