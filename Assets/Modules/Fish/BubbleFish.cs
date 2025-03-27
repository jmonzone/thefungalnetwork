using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class BubbleFish : NetworkBehaviour
{
    [SerializeField] private Bubble bubblePrefab;
    [SerializeField] private float pitch = 2f;
    [SerializeField] private AudioClip audioClip;

    private Fish fish;
    private Animator animator;
    private AudioSource audioSource;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        fish = GetComponent<Fish>();

        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowComplete += () => InflateServerRpc(NetworkObject.OwnerClientId, throwFish.TargetPosition);

        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    [ServerRpc]
    private void InflateServerRpc(ulong clientId, Vector3 targetPosition)
    {
        var bubble = Instantiate(bubblePrefab, targetPosition, Quaternion.identity);;
        bubble.NetworkObject.SpawnWithOwnership(clientId);

        InflateClientRpc(bubble.NetworkObjectId);
    }

    [ClientRpc]
    private void InflateClientRpc(ulong bubbleObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(bubbleObjectId, out var bubbleObject))
        {
            var bubble = bubbleObject.GetComponent<Bubble>();

            animator.Play("Jump");

            if (IsOwner)
            {
                bubble.StartInflate(fish.PickedUpFungalId.Value);
                Invoke(nameof(ReturnFish), 1f);
            }
        }
    }

    private void ReturnFish()
    {
        fish.ReturnToRadialMovement();
    }
}
