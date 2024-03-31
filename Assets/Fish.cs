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
        movementController.OnDestinationReached += () =>
        {
            movementController.Speed = 3f;
            OnCaught?.Invoke();
        };
    }

    public void Attract(Transform bob)
    {
        movementController.SetTarget(bob);
    }

    public void Scare(Vector3 bobPosition)
    {
        isCatchable = false;

        var direction = transform.position - bobPosition;
        direction.y = 0;
        movementController.SetDirection(direction);

    }
}
