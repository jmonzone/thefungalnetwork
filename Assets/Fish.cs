using GURU.Entities;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MovementController))]
public class Fish : MonoBehaviour
{
    [SerializeField] private bool isCatchable = true;

    private MovementController movementController;
    public bool IsCatchable => isCatchable;

    public event UnityAction OnCaught;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        
    }

    public void Attract(Transform bob)
    {
        movementController.SetTarget(bob);
        movementController.OnDestinationReached += OnDestinationReached;
    }

    private void OnDestinationReached()
    {
        movementController.Speed = 5f;
        OnCaught?.Invoke();

        movementController.OnDestinationReached -= OnDestinationReached;
    }

    public void Scare(Vector3 bobPosition)
    {
        isCatchable = false;

        var direction = transform.position - bobPosition;
        direction.y = 0;
        movementController.SetDirection(direction);

        movementController.Speed = 2f;
    }
}
