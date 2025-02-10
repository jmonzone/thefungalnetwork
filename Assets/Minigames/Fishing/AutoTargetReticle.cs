using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoTargetReticle : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private Transform reticle;
    [SerializeField] private float searchRadius = 3f;

    private void Update()
    {
        if (!playerReference.Transform) return;

        // Find all FishController objects within the search radius
        Collider[] colliders = Physics.OverlapSphere(playerReference.Transform.position, searchRadius);
        FishController closestFish = colliders
            .Select(c => c.GetComponentInParent<FishController>())
            .Where(fish => fish != null)
            .OrderBy(fish => Vector3.Distance(playerReference.Transform.position, fish.transform.position))
            .FirstOrDefault();

        if (closestFish)
        {
            reticle.gameObject.SetActive(true);
            reticle.position = closestFish.transform.position;
        }
        else
        {
            reticle.gameObject.SetActive(false);
        }
    }
}
