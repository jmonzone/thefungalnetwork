using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindFish : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float hitStun = 0.5f;

    private FishController fish;

    private HitDetector hitDetector;

    private void Awake()
    {
        fish = GetComponent<FishController>();
        hitDetector = GetComponent<HitDetector>();
    }

    private void Start()
    {
        fish.ThrowFish.OnThrowStart += ThrowFish_OnThrowStart;
    }

    private void ThrowFish_OnThrowStart(Vector3 arg0)
    {
        StartCoroutine(ThrowFishUpdate());
    }

    public IEnumerator ThrowFishUpdate()
    {
        var sourceFungal = fish.Fungal.Id;

        var hasHit = false;

        List<Collider> hits = new List<Collider>();

        while (!hasHit)
        {
            hitDetector.CheckHits(1f, hit =>
            {
                if (hits.Contains(hit)) return;

                var targetFungal = hit.GetComponent<FungalController>();

                if (targetFungal == null) return;
                if (targetFungal == fish.Fungal) return;
                if (targetFungal.IsDead) return;

                //targetFungal.ModifySpeedServerRpc(0f, hitStun, showStunAnimation: false);
                targetFungal.Health.Damage(damage, sourceFungal);
                Debug.Log($"WindFish damage {targetFungal.name}");

                hits.Add(hit);
                hasHit = true;
            });

            yield return null;
        }
    }
}
