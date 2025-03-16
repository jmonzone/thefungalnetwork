using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Pufferfish : NetworkBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float minExplosionRadius = 3f;
    [SerializeField] private float maxExplosionRadius = 3f;
    [SerializeField] private float minExplosionDamage = 1f;
    [SerializeField] private float maxExplosionDamage = 3f;

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
        fish.OnPrepareThrow += StartTemperServerRpc;

        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowStart += ThrowFish_OnThrowStart;
        throwFish.OnThrowComplete += OnThrowComplete;

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

    private void ThrowFish_OnThrowStart(Vector3 targetPosition)
    {
        if (IsOwner) OnThrowStartServerRpc(targetPosition, ExplosionRadius);
    }

    [ServerRpc]
    public void OnThrowStartServerRpc(Vector3 targetPosition, float radius)
    {
        OnThrowStartClientRpc(targetPosition, radius);
    }

    [ClientRpc]
    private void OnThrowStartClientRpc(Vector3 targetPosition, float radius)
    {
        pufferfishExplosion.ShowHitIndicator(targetPosition, radius);
    }

    private void OnThrowComplete()
    {
        if (IsOwner) Explode();
    }

    private void HandleExplodeComplete()
    {
        if (IsOwner)
        {
            StartCoroutine(RespawnRoutine());
            StopTemperServerRpc();
        }
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(1f);
        fish.ReturnToRadialMovement();
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

    public float ExplosionRadius => pufferfishTemper.Temper * (maxExplosionRadius - minExplosionRadius) + minExplosionRadius;
    private void Explode()
    {
        movement.Stop();
        float damage = pufferfishTemper.Temper * (maxExplosionDamage - minExplosionDamage) + minExplosionDamage;
        pufferfishExplosion.DealExplosionDamage(damage, ExplosionRadius);
        ExplodeServerRpc(ExplosionRadius);
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