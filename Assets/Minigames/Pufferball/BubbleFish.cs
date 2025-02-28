using Unity.Netcode;
using UnityEngine;

public class BubbleFish : NetworkBehaviour
{
    [SerializeField] private NetworkObject bubblePrefab;

    private Fish fish;

    private void Awake()
    {
        fish = GetComponent<Fish>();

        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowComplete += OnThrowComplete;
    }

    private void OnThrowComplete()
    {
        if (IsOwner)
        {
            fish.ReturnToRadialMovement();
            RequestSpawnBubbleServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnBubbleServerRpc()
    {
        var bubble = Instantiate(bubblePrefab, transform.position, Quaternion.identity);
        bubble.Spawn();
    }
}
