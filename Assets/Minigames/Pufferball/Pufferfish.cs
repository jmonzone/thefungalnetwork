using UnityEngine;

public class Pufferfish : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;

    private Movement movement;
    private PufferfishSling pufferfishSling;
    private PufferfishExplosion pufferfishExplosion;
    private PufferfishTemper pufferfishTemper;

    public bool IsCaught { get; private set; } = false;

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
        pufferfishExplosion.Explode();
    }
}
