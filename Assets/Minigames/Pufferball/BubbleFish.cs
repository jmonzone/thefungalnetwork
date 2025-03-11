using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class BubbleFish : NetworkBehaviour
{
    [SerializeField] private Bubble bubblePrefab;

    private Fish fish;
    private Animator animator;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        fish = GetComponent<Fish>();

        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowComplete += InflateServerRpc;

        animator = GetComponentInChildren<Animator>();
    }

    [ServerRpc]
    private void InflateServerRpc()
    {
        var bubble = Instantiate(bubblePrefab, transform.position, Quaternion.identity);
        bubble.NetworkObject.Spawn();

        InflateClientRpc();
    }

    [ClientRpc]
    private void InflateClientRpc()
    {
        animator.Play("Jump");

        if (IsOwner) StartCoroutine(InflateRoutine());
    }

    private IEnumerator InflateRoutine()
    {
        yield return new WaitForSeconds(1f);
        fish.ReturnToRadialMovement();
    }
}
