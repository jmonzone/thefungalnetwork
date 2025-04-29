using UnityEngine;
using UnityEngine.Events;

public class WaterFish : MonoBehaviour
{
    [SerializeField] private BubbleController bubblePrefab;

    private FishController fish;
    private Animator animator;

    public event UnityAction<Vector3> OnSpawnBubble;

    private void Awake()
    {
        fish = GetComponent<FishController>();
        animator = GetComponentInChildren<Animator>();

        OnSpawnBubble += HandleSpawnBubble;
    }

    private void Start()
    {
        fish.ThrowFish.OnTrajectoryComplete += () => OnSpawnBubble?.Invoke(fish.ThrowFish.TargetPosition);
    }

    public void HandleSpawnBubble(Vector3 targetPosition)
    {
        CreateBubble(targetPosition);
    }

    public BubbleController CreateBubble(Vector3 targetPosition)
    {
        var bubble = Instantiate(bubblePrefab, targetPosition, Quaternion.identity);

        HandleBubbleSpawn();
        return bubble;
    }

    public void HandleBubbleSpawn()
    {
        animator.Play("Jump");
        Invoke(nameof(ReturnFish), 1f);

    }
    private void ReturnFish()
    {
        fish.Respawn();
    }
}
