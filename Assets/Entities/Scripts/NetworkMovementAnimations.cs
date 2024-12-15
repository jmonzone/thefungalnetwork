using Unity.Netcode;
using UnityEngine;

public class NetworkMovementAnimations : NetworkBehaviour
{
    [SerializeField] private MovementAnimations animations;

    private NetworkVariable<bool> isMovingNetwork = new NetworkVariable<bool>();
    private NetworkVariable<float> directionMagnitudeNetwork = new NetworkVariable<float>();
    private NetworkVariable<float> animationSpeedNetwork = new NetworkVariable<float>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        animations.enabled = IsOwner;
    }

    private void Update()
    {
        if (IsOwner)
        {
            UpdateAnimationStateServerRpc(animations.IsMoving, animations.DirectionMagnitude, animations.AnimationSpeed);
        }
        else
        {
            SyncAnimations();
        }
    }

    [ServerRpc]
    private void UpdateAnimationStateServerRpc(bool isMoving, float directionMagnitude, float animationSpeed)
    {
        // Update network variables
        isMovingNetwork.Value = isMoving;
        directionMagnitudeNetwork.Value = directionMagnitude;
        animationSpeedNetwork.Value = animationSpeed;
    }

    private void SyncAnimations()
    {
        animations.Animator.SetBool("isMoving", isMovingNetwork.Value);
        animations.Animator.speed = animationSpeedNetwork.Value;
        if (isMovingNetwork.Value)
        {
            animations.Animator.speed *= directionMagnitudeNetwork.Value / 1.5f;
        }
    }
}
