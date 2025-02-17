using UnityEngine;

public class Pufferfish : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;

    private Movement movement;
    private PufferfishSling pufferfishSling;
    private PufferfishExplosion pufferfishExplosion;

    private void Awake()
    {
        movement = GetComponent<Movement>();

        pufferfishSling = GetComponent<PufferfishSling>();
        pufferfishSling.OnSlingComplete += () =>
        {
            pufferfishExplosion.Explode();
        };

        pufferfishExplosion = GetComponent<PufferfishExplosion>();
        pufferfishExplosion.OnExplodeComplete += () =>
        {
            movement.SetSpeed(5);
            movement.StartRadialMovement(true);
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
    }

    public void Sling(Vector3 direction)
    {
        pufferfishSling.Sling(direction);
    }
}
