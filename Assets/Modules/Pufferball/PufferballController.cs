using UnityEngine;

public class PufferballController :  MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Wall")
        {
            var contactPoint = collision.contacts[0];

            var movement = GetComponent<MovementController>();
            var direction = movement.Direction;

            if (Mathf.Abs(contactPoint.normal.x) > 0.01f) direction.x *= -Mathf.Abs(contactPoint.normal.x);
            if (Mathf.Abs(contactPoint.normal.z) > 0.01f) direction.z *= -Mathf.Abs(contactPoint.normal.z);

            movement.SetDirection(direction);
        }
    }
}
