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

        InflateClientRpc();
    }

    [ClientRpc]
    private void InflateClientRpc()
    {
        animator.Play("Jump");
        //audioSource.clip = audioClip;
        //audioSource.pitch = pitch;
        //audioSource.Play();

        if (IsOwner) StartCoroutine(InflateRoutine());
    }

    private IEnumerator InflateRoutine()
    {
        yield return new WaitForSeconds(1f);
        fish.ReturnToRadialMovement();
    }
}
