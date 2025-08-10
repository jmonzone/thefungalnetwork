using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UnitMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private Transform lookTransform;

    public bool IsMoving => isMoving;

    public event UnityAction<bool> OnIsMovingHasChanged;

    public void StartMovement(Vector3 targetPosition, UnityAction onComplete = null)
    {
        Debug.Log("StartMovement");

        StopAllCoroutines();
        StartCoroutine(MoveToPosition(targetPosition, onComplete));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, UnityAction onComplete = null)
    {
        isMoving = true;
        OnIsMovingHasChanged?.Invoke(isMoving);

        while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            var direction = targetPosition - transform.position;
            direction.y = 0;
            transform.position += movementSpeed * Time.deltaTime * direction.normalized;

            if (direction.magnitude > 0) lookTransform.forward = direction;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
        OnIsMovingHasChanged?.Invoke(isMoving);

        onComplete?.Invoke();
    }
}
