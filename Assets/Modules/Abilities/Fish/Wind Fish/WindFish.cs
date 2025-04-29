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
        fish.ThrowFish.OnTrajectoryStart += ThrowFish_OnThrowStart;
        fish.ThrowFish.OnTrajectoryComplete += ThrowFish_OnThrowComplete;
    }

    private void ThrowFish_OnThrowComplete()
    {
        StopAllCoroutines();
    }

    private void ThrowFish_OnThrowStart(Vector3 arg0)
    {
        StartCoroutine(ThrowFishUpdate());
    }

    public IEnumerator ThrowFishUpdate()
    {
        List<FungalController> hits = new List<FungalController>();

        while (true)
        {
            hitDetector.CheckFungalHits(1f, damage, hitStun, fish.Fungal,
                onHit: hit =>
                {
                    hits.Add(hit);
                },
                isValid: (fungal) =>
                {
                    return !hits.Contains(fungal);
                });
            
            yield return null;
        }
    }
}
