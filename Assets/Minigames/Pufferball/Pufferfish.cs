using Unity.Netcode;
using UnityEngine;

public class Pufferfish : NetworkBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float minExplosionRadius = 3f;
    [SerializeField] private float maxExplosionRadius = 3f;

    private Movement movement;
    private PufferfishSling pufferfishSling;
    private PufferfishExplosion pufferfishExplosion;
    private PufferfishTemper pufferfishTemper;

    public bool IsCaught { get; private set; }

    private void Awake()
    {
        movement = GetComponent<Movement>();

        pufferfishSling = GetComponent<PufferfishSling>();
        pufferfishSling.OnSlingComplete += () =>
        {
            Explode();
        };

        pufferfishExplosion = GetComponent<PufferfishExplosion>();
        pufferfishExplosion.OnExplodeComplete += () =>
        {
            pufferfishTemper.StopTimer();
            movement.SetSpeed(5);
            movement.StartRadialMovement(true);
        };

        pufferfishTemper = GetComponent<PufferfishTemper>();
        pufferfishTemper.OnMaxTemperReached += () =>
        {
            Explode();
        };
    }

    public void Catch(Transform bobber)
    {
        movement.SetSpeed(10);
        movement.Follow(bobber); // Follow the bobber
        RequestCatchServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestCatchServerRpc(ulong requestingClientId)
    {
        NetworkObject.ChangeOwnership(requestingClientId);
    }

    public void PickUp()
    {
        movement.Follow(playerReference.Transform); // Follow the player
        pufferfishTemper.StartTemper();
        IsCaught = true;
    }

    public void Sling(Vector3 direction)
    {
        pufferfishSling.Sling(direction);
    }

    private void Explode()
    {
        movement.Stop();
        IsCaught = false;
        pufferfishExplosion.Explode(pufferfishTemper.Temper * (maxExplosionRadius - minExplosionRadius) + minExplosionRadius);
    }
}
