using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkWindFish : NetworkBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float hitStun = 0.5f;
    [SerializeField] private GameObject trailRenderer;
    [SerializeField] private Renderer fishRenderer;
    [SerializeField] private Material empoweredMaterial;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float audioPitch = 1.5f;

    private FishController fish;
    private HitDetector hitDetector;
    private AudioSource audioSource;
    private Material originalMaterial;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        fish = GetComponent<FishController>();
        hitDetector = GetComponent<HitDetector>();
        audioSource = GetComponent<AudioSource>();

        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowStart += OnThrowStartServerRpc;
        throwFish.OnThrowComplete += OnThrowCompleteServerRpc;

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

        if (IsOwner)
        {
            StartCoroutine(ThrowFishUpdate());
        }
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

    private IEnumerator ThrowFishUpdate()
    {
        var sourceFungal = fish.Fungal.Id;

        var hasHit = false;

        List<Collider> hits = new List<Collider>();

        while (!hasHit)
        {
            hitDetector.CheckHits(1f, hit =>
            {
                if (hits.Contains(hit)) return;

                var targetFungal = hit.GetComponent<NetworkFungal>();

                if (targetFungal == null) return;
                if (targetFungal.Fungal == fish.Fungal) return;
                if (targetFungal.IsDead) return;

                targetFungal.ModifySpeedServerRpc(0f, hitStun, showStunAnimation: false);
                targetFungal.Health.Damage(damage, sourceFungal);
                Debug.Log($"WindFish damage {targetFungal.name}");

                hits.Add(hit);
                hasHit = true;
            });

            yield return null; ;
        }
    }
}
