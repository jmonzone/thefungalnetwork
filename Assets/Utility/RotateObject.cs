using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private Vector3 axis;
    [SerializeField] private float rotationSpeed = 50f;

    private void Update()
    {
        transform.Rotate(axis, rotationSpeed * Time.deltaTime);
    }
}
