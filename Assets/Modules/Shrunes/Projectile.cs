using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private MovementController movement;

    public void Shoot(Vector3 direction)
    {
        movement.SetPosition(transform.position + direction.normalized * 5f, () =>
        {
            gameObject.SetActive(false);
        });
    }
}
