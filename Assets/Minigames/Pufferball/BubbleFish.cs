using Unity.Netcode;
using UnityEngine;

public class BubbleFish : NetworkBehaviour
{
    [SerializeField] private NetworkObject bubblePrefab;

    private void Awake()
    {
        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowComplete += OnThrowComplete;
    }

    private void OnThrowComplete()
    {
        if (IsOwner)
        {
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
