using Unity.Netcode;
using UnityEngine;

public class NetworkWindFish : NetworkBehaviour
{
    [SerializeField] private GameObject trailRenderer;
    [SerializeField] private Renderer fishRenderer;
    [SerializeField] private Material empoweredMaterial;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float audioPitch = 1.5f;

    private AudioSource audioSource;
    private Material originalMaterial;

    private WindFish windFish;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        windFish = GetComponent<WindFish>();

        audioSource = GetComponent<AudioSource>();

        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnTrajectoryStart += OnThrowStartServerRpc;
        throwFish.OnTrajectoryComplete += OnThrowCompleteServerRpc;

        originalMaterial = fishRenderer.material;
    }

    [ServerRpc]
    private void OnThrowStartServerRpc(Vector3 targetPosition)
    {
        OnThrowStartClientRpc();
    }

    [ClientRpc]
    private void OnThrowStartClientRpc()
    {
        trailRenderer.SetActive(true);
        fishRenderer.material = empoweredMaterial;
        audioSource.clip = audioClip;
        audioSource.pitch = audioPitch;
        audioSource.Play();
    }

    [ServerRpc]
    private void OnThrowCompleteServerRpc()
    {
        OnThrowCompleteClientRpc();
    }

    [ClientRpc]
    private void OnThrowCompleteClientRpc()
    {
        trailRenderer.SetActive(false);
        fishRenderer.material = originalMaterial;

        if (IsOwner)
        {
            StopAllCoroutines();
        }
    }
}
