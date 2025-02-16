using UnityEngine;

public class FishingRodCatch : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 1f;

    private Transform caughtPufferfish;

    private void Update()
    {
        if (caughtPufferfish == null)
        {
            DetectPufferfish();
        }
        else
        {
            MovePufferfish();
        }
    }

    private void OnDisable()
    {
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
                caughtPufferfish = pufferfish.transform;
                break;
            }
        }
    }

    private void MovePufferfish()
    {
        caughtPufferfish.position = Vector3.Lerp(caughtPufferfish.position, transform.position, Time.deltaTime * 5f);
    }
}
