using UnityEngine;

public class Pufferfish : MonoBehaviour
{
    [SerializeField] private Movement movement;
    [SerializeField] private PlayerReference playerReference;

    public void Catch(Transform bobber)
    {
        movement.SetFollow(bobber);
    }

    public void PickUp()
    {
        movement.SetFollow(playerReference.Transform);
    }

    public void Sling(Vector3 direction)
    {
        var targetPosition = transform.position + direction * 4f;
        targetPosition.y = 0;
        movement.SetMoveTo(targetPosition);
    }
}
