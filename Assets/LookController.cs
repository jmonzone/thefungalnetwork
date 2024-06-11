using UnityEngine;

public class LookController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 3f;

    public Vector3 Direction { get; set; }

    private void Update()
    {
        transform.forward = Vector3.Lerp(transform.forward, Direction, rotationSpeed * Time.deltaTime);
    }
}
