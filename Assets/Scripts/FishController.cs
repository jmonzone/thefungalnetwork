using GURU.Entities;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MovementController))]
public class FishController : MonoBehaviour
{
    [SerializeField] private FishData data;
    [SerializeField] private bool isTreasure = false;
    [SerializeField] private bool isCatchable;

    private MovementController movementController;
    public FishData Data => data;
    public bool IsTreasure => isTreasure;
    public bool IsCaught { get; private set; }
    public bool IsCatchable { get => isCatchable; set => isCatchable = value; }

    public event UnityAction OnCaught;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
    }

    public void SetData(FishData data)
    {
        this.data = data;
    }

    public void Attract(Transform bob)
    {
        movementController.Speed = 2f;
        movementController.SetTarget(bob);
        movementController.OnDestinationReached += HandleCapture;
    }

    private void HandleCapture()
    {
        IsCaught = true;
        movementController.Speed = 6f;
        movementController.normalizeSpeed = false;
        OnCaught?.Invoke();
        movementController.OnDestinationReached -= HandleCapture;
    }

    public void Scare(Vector3 bobPosition)
    {
        if (isTreasure) return;

        IsCaught = false;

        var direction = transform.position - bobPosition;
        direction.y = 0;
        transform.forward = direction;
        movementController.SetDirection(direction);

        movementController.Speed = 2f;
    }
}
