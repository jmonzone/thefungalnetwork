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
        pufferfishSling.OnSlingComplete += () =>
        {
            if (IsOwner) Explode();
        };

        pufferfishExplosion = GetComponent<PufferfishExplosion>();
        pufferfishExplosion.OnExplodeComplete += () =>
        {
            if (IsOwner)
            {
                StopTemperServerRpc();
                movement.SetSpeed(5);
                movement.StartRadialMovement(true);
            }
        };

        pufferfishTemper = GetComponent<PufferfishTemper>();
        pufferfishTemper.OnMaxTemperReached += () =>
        {
            if (IsOwner)
            {
                Debug.Log("OnMaxTemperReached");
                Explode();
                OnMaxTemperReached?.Invoke();
            }
        };
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
        Temper.OnValueChanged += (oldValue, newValue) => pufferfishTemper.SetTemper(newValue);
    }

    public override void OnNetworkDespawn()
    {
        Temper.OnValueChanged -= (oldValue, newValue) => pufferfishTemper.SetTemper(newValue);
    }

    public void Catch(Transform bobber)
    {
        movement.SetSpeed(10);
        movement.Follow(bobber); // Follow the bobber
        RequestOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void PickUp()
    {
        movement.Follow(playerReference.Transform); // Follow the player
        StartTemperServerRpc();
    }

    public void Sling(Vector3 direction)
    {
        pufferfishSling.Sling(direction);
    }

    private void Explode()
    {
        movement.Stop();
        ExplodeServerRpc(pufferfishTemper.Temper * (maxExplosionRadius - minExplosionRadius) + minExplosionRadius);
    }

    [ServerRpc]
    public void StartTemperServerRpc()
    {
        pufferfishTemper.StartTemper();
    }

    [ServerRpc]
    public void StopTemperServerRpc()
    {
        pufferfishTemper.StopTimer();
    }

    [ServerRpc]
    public void ExplodeServerRpc(float radius)
    {
        ExplodeClientRpc(radius);
    }

    [ClientRpc]
    private void ExplodeClientRpc(float radius)
    {
        pufferfishExplosion.Explode(radius);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong requestingClientId)
    {
        NetworkObject.ChangeOwnership(requestingClientId);
    }
}
