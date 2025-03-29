using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class WindFish : NetworkBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float hitStun = 0.5f;
    [SerializeField] private GameObject trailRenderer;
    [SerializeField] private Renderer fishRenderer;
    [SerializeField] private Material empoweredMaterial;

    private HitDetector hitDetector;
    private ulong sourceFungal;
    private Fish fish;
    private Material originalMaterial;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        fish = GetComponent<Fish>();
        hitDetector = GetComponent<HitDetector>();
        originalMaterial = fishRenderer.material;

        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowStart += ThrowFish_OnThrowStart;
        throwFish.OnThrowComplete += ThrowFish_OnThrowComplete;
    }

    // todo make networked
    private void ThrowFish_OnThrowStart(Vector3 arg0)
    {
        sourceFungal = fish.PickedUpFungalId.Value;
        trailRenderer.SetActive(true);
        fishRenderer.material = empoweredMaterial;
        StartCoroutine(ThrowFishUpdate());
    }

    private void ThrowFish_OnThrowComplete()
    {
        trailRenderer.SetActive(false);
        fishRenderer.material = originalMaterial;
    }

    private IEnumerator ThrowFishUpdate()
    {
        var hasHit = false;

        List<Collider> hits = new List<Collider>();

        while (!hasHit)
        {
            hitDetector.CheckHits(1f, hit =>
            {
                if (hits.Contains(hit)) return;

                var targetFungal = hit.GetComponent<NetworkFungal>();

                if (targetFungal == null) return;
                if (targetFungal.NetworkObjectId == sourceFungal) return;
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
