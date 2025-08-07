using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private Transform lookTransform;

    public bool IsMoving => isMoving;

    public event UnityAction<bool> OnIsMovingHasChanged;

    public void MoveToPosition(Vector3 targetPosition)
    {
        StopAllCoroutines();
        StartCoroutine(MovementRoutine(targetPosition));
    }

    private IEnumerator MovementRoutine(Vector3 targetPosition)
    {
        isMoving = true;
        OnIsMovingHasChanged?.Invoke(isMoving);

        while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            var direction = targetPosition - transform.position;
            direction.y = 0;
            transform.position += movementSpeed * Time.deltaTime * direction.normalized;

            if (direction != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(direction);
                lookTransform.rotation = Quaternion.Slerp(lookTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
        OnIsMovingHasChanged?.Invoke(isMoving);
    }
}
