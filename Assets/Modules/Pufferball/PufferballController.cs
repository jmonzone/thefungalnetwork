using UnityEngine;

public class PufferballController :  MonoBehaviour
{
    [SerializeField] private LayerMask wallLayer;

    public MovementController Movement { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<MovementController>();
        Spawn();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (wallLayer.Contains(collision.gameObject.layer))
        {
            var contactPoint = collision.contacts[0];

            var targetDirection = Movement.Direction;

            if (Mathf.Abs(contactPoint.normal.x) > 0.01f) targetDirection.x *= -Mathf.Abs(contactPoint.normal.x);
            if (Mathf.Abs(contactPoint.normal.z) > 0.01f) targetDirection.z *= -Mathf.Abs(contactPoint.normal.z);

            Movement.SetDirection(targetDirection);
        }
    }

    public void Spawn()
    {
        Debug.Log("spawning");
        transform.position = new Vector3(0, 3, 0);
        gameObject.SetActive(true);

        var targetDirection = Random.onUnitSphere;
        targetDirection.y = 0;

        Movement.SetDirection(targetDirection.normalized);
    }
}
