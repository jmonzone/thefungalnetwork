using Unity.Netcode;
using UnityEngine.Events;

public class Pufferfish : NetworkBehaviour
{
    private Movement movement;
    private FishController fish;
    private PufferfishExplosion pufferfishExplosion;
    private PufferfishTemper pufferfishTemper;
    private ThrowFish throwFish;

    public event UnityAction OnMaxTemperReached;

    public NetworkVariable<float> Temper = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        movement = GetComponent<Movement>();

        fish = GetComponent<FishController>();
        fish.OnPrepareThrow += StartTemperServerRpc;

        throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowComplete += OnThrowComplete;

        pufferfishExplosion = GetComponent<PufferfishExplosion>();
        pufferfishExplosion.OnExplodeComplete += HandleExplodeComplete;

        pufferfishTemper = GetComponent<PufferfishTemper>();
        pufferfishTemper.OnMaxTemperReached += HandleMaxTemperReached;
    }

    private void Update()
    {
        if (IsOwner)
        {
            //throwFish.SetRadius(ExplosionRadius);
        }

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

    private void OnThrowComplete()
    {
        if (IsOwner) Explode();
    }

    private void HandleExplodeComplete()
    {
        if (IsOwner)
        {
            //StartCoroutine(RespawnRoutine());
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
        if (IsOwner)
        {
            movement.Stop();
            //ExplodeServerRpc(ExplosionRadius);
        }
    }

    [ServerRpc]
    public void ExplodeServerRpc(float radius)
    {
        ExplodeClientRpc(radius);
    }

    [ClientRpc]
    private void ExplodeClientRpc(float radius)
    {
        //pufferfishExplosion.StartExplosionAnimation(radius);
        //float damage = pufferfishTemper.Temper * (maxExplosionDamage - minExplosionDamage) + minExplosionDamage;
        //if (IsOwner) pufferfishExplosion.EnableDamage(damage);
    }
}