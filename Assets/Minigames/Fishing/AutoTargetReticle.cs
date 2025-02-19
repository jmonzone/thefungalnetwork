using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AutoTargetReticle : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private Transform reticle;
    [SerializeField] private float searchRadius = 3f;

    public FishController TargetFishController { get; private set; } 

    private void Update()
    {
        if (!playerReference.Movement) return;

        // Find all FishController objects within the search radius
        Collider[] colliders = Physics.OverlapSphere(playerReference.Movement.transform.position, searchRadius);
        TargetFishController = colliders
            .Select(c => c.GetComponentInParent<FishController>())
            .Where(fish => fish != null)
            .OrderBy(fish => Vector3.Distance(playerReference.Movement.transform.position, fish.transform.position))
            .FirstOrDefault();

        if (TargetFishController)
        {
            reticle.gameObject.SetActive(true);
            reticle.position = TargetFishController.transform.position;
        }
        else
        {
            reticle.gameObject.SetActive(false);
        }
    }
}
