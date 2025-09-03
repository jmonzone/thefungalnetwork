using UnityEngine;

public class PhotoController : MonoBehaviour
{
    //[Header("Target Settings")]
    //public Transform 
    //public Transform target;            // The guest to look at
    //public Vector3 offset = new Vector3(0, 2, -5); // Camera offset relative to target

    //[Header("Smoothing Settings")]
    //public float followSpeed = 5f;      // How fast the camera moves to follow
    //public float rotationSpeed = 5f;    // How fast the camera rotates to look at the target

    //void LateUpdate()
    //{
    //    if (!target) return;

    //    // Desired position with offset
    //    Vector3 desiredPosition = target.position + offset;

    //    // Smoothly move camera
    //    transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

    //    // Smoothly rotate to look at target
    //    Vector3 direction = target.position - transform.position;
    //    Quaternion desiredRotation = Quaternion.LookRotation(direction);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    //}

    //// Optional: change target at runtime
    //public void SetTarget(Transform newTarget)
    //{
    //    target = newTarget;
    //}
}
