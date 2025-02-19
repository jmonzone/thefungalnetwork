using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Pufferfish : NetworkBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float minExplosionRadius = 3f;
    [SerializeField] private float maxExplosionRadius = 3f;

    private Movement movement;
    private PufferfishSling pufferfishSling;
    private PufferfishExplosion pufferfishExplosion;
    private PufferfishTemper pufferfishTemper;

    public event UnityAction OnMaxTemperReached;

    public NetworkVariable<float> Temper = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        movement = GetComponent<Movement>();

        pufferfishSling = GetComponent<PufferfishSling>();
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
            StopTemperServerRpc();
            movement.SetSpeed(5);
            movement.StartRadialMovement(true);
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

    public void Catch(Transform bobber)
    {
        movement.SetSpeed(10);
        movement.Follow(bobber);
        RequestCatchServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void PickUp()
    {
        movement.Follow(playerReference.Movement.transform);
        RequestPickUpServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void Sling(Vector3 direction)
    {
        pufferfishSling.Sling(direction);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestCatchServerRpc(ulong requestingClientId)
    {
        NetworkObject.ChangeOwnership(requestingClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestPickUpServerRpc(ulong clientId)
    {
        NetworkObject.ChangeOwnership(clientId);
        StartTemperServerRpc();
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