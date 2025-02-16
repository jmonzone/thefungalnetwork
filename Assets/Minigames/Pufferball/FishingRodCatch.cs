using UnityEngine;

public class FishingRodCatch : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float detectionRadius = 1f;

    private Pufferfish caughtPufferfish;

    private void Update()
    {
        if (caughtPufferfish == null)
        {
            DetectPufferfish();
        }
    }

    private void OnDisable()
    {
        if (caughtPufferfish) caughtPufferfish.PickUp(playerReference.Transform);
        caughtPufferfish = null;
    }

    private void DetectPufferfish()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider hit in hits)
        {
            Pufferfish pufferfish = hit.GetComponentInParent<Pufferfish>();
            if (pufferfish != null)
            {
                caughtPufferfish = pufferfish;
                pufferfish.Catch(transform);
                break;
            }
        }
    }
}
