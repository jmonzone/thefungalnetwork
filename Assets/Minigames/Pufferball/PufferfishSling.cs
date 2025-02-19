using UnityEngine;
using UnityEngine.Events;

public class PufferfishSling : MonoBehaviour
{
    [SerializeField] private float range = 5f;

    private Movement movement;

    public event UnityAction OnSlingComplete;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        enabled = false;
    }

    private void OnEnable()
    {
        movement.OnDestinationReached += Movement_OnDestinationReached;
    }

    private void OnDisable()
    {
        movement.OnDestinationReached -= Movement_OnDestinationReached;
    }

    public void Sling(Vector3 targetPosition)
    {
        enabled = true;
        targetPosition.y = 0; // Keep on the ground
        movement.SetTargetPosition(targetPosition); // Move towards the target
    }

    private void Movement_OnDestinationReached()
    {
        enabled = false;
        OnSlingComplete?.Invoke();
    }
}
