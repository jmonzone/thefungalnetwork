using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RandomMovement))]
public class FishController : MonoBehaviour
{
    [SerializeField] private FishData data;
    [SerializeField] private bool isTreasure = false;
    [SerializeField] private bool isCatchable;

    public FishData Data => data;
    public bool IsTreasure => isTreasure;
    public bool IsCaught { get; private set; }
    public bool IsCatchable { get => isCatchable; set => isCatchable = value; }
    public bool IsAttacted { get; private set; }

    public event UnityAction OnCaught;

    private void OnEnable()
    {
        transform.forward = Utility.RandomXZVector;
    }

    public void Initialize(FishData data, Collider bounds)
    {
        this.data = data;

        var randomMovement = GetComponent<RandomMovement>();
        randomMovement.SetBounds(bounds);
    }

    public void Attract(Transform bob)
    {
        IsAttacted = true;
    }

    public void Catch()
    {
        IsCaught = true;
        OnCaught?.Invoke();
        gameObject.SetActive(false);
    }

    private void HandleCapture()
    {
        IsCaught = true;
        OnCaught?.Invoke();
    }

    public void Scare(Vector3 bobPosition)
    {
        if (isTreasure) return;

        IsCaught = false;
        IsAttacted = false;

        var direction = transform.position - bobPosition;
        direction.y = 0;
        transform.forward = direction;
    }
}
