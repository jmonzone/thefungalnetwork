using Unity.Netcode;
using UnityEngine;

public class MovementAnimations : NetworkBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private Animator animator;
    [SerializeField] private float animationSpeed = 1;

    public Animator Animator => animator;

    private NetworkVariable<bool> isMovingNetwork = new NetworkVariable<bool>();
    private NetworkVariable<float> directionMagnitudeNetwork = new NetworkVariable<float>();

    private void Awake()
    {
        if (!animator) enabled = false;
        movementController = GetComponentInParent<MovementController>();
    }

    private bool IsOffline()
    {
        // Check if the networking system is inactive
        return NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening;
    }


    private void Update()
    {
        if (IsOwner || IsOffline())
        {
            UpdateAnimationParameters();
        }
        else
        {
            SyncAnimations();
        }
    }

    private void UpdateAnimationParameters()
    {
        // Calculate local animation parameters
        var isMoving = !movementController.IsAtDestination;
        var directionMagnitude = movementController.Direction.magnitude;

        // Set parameters locally
        animator.SetBool("isMoving", isMoving);
        animator.speed = animationSpeed;
        if (isMoving) animator.speed *= directionMagnitude / 1.5f;

        // Sync parameters to other clients
        UpdateAnimationStateServerRpc(isMoving, directionMagnitude);
    }

    private void SyncAnimations()
    {
        animator.SetBool("isMoving", isMovingNetwork.Value);
        animator.speed = animationSpeed;
        if (isMovingNetwork.Value)
            animator.speed *= directionMagnitudeNetwork.Value / 1.5f;
    }

    [ServerRpc]
    private void UpdateAnimationStateServerRpc(bool isMoving, float directionMagnitude)
    {
        // Update network variables
        isMovingNetwork.Value = isMoving;
        directionMagnitudeNetwork.Value = directionMagnitude;
    }
}
