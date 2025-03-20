using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class BubbleFish : NetworkBehaviour
{
    [SerializeField] private Bubble bubblePrefab;
    [SerializeField] private AudioClip audioClip;

    private Fish fish;
    private Animator animator;
    private AudioSource audioSource;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        fish = GetComponent<Fish>();

        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowComplete += () => InflateServerRpc(NetworkObject.OwnerClientId);

        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    [ServerRpc]
    private void InflateServerRpc(ulong clientId)
    {
        var bubble = Instantiate(bubblePrefab, transform.position, Quaternion.identity);
        bubble.NetworkObject.SpawnWithOwnership(clientId);

        InflateClientRpc();
    }

    [ClientRpc]
    private void InflateClientRpc()
    {
        animator.Play("Jump");
        audioSource.clip = audioClip;
        audioSource.pitch = 0.5f;
        audioSource.Play();

        if (IsOwner) StartCoroutine(InflateRoutine());
    }

    private IEnumerator InflateRoutine()
    {
        yield return new WaitForSeconds(1f);
        fish.ReturnToRadialMovement();
    }
}
