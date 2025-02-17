using System.Collections;
using UnityEngine;

public class FishingRodProjectile : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float speed = 7.5f;
    [SerializeField] private float range = 5f;
    [SerializeField] private float detectionRadius = 1f;

    public Pufferfish Pufferfish { get; private set; }

    private void Update()
    {
        if (Pufferfish == null)
        {
            DetectPufferfish();
        }
    }

    private void OnDisable()
    {
        if (Pufferfish) Pufferfish.PickUp();
    }

    private void DetectPufferfish()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider hit in hits)
        {
            Pufferfish = hit.GetComponentInParent<Pufferfish>();
            if (Pufferfish != null)
            {
                Pufferfish.Catch(transform);
                break;
            }
        }
    }

    public void CastFishingRod(Vector3 direction)
    {
        if (Pufferfish)
        {
            Pufferfish.Sling(direction);
            Pufferfish = null;
        }
        else
        {
            gameObject.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(MoveProjectile(direction));
        }
    }

    private IEnumerator MoveProjectile(Vector3 direction)
    {
        Vector3 startPos = playerReference.Transform.position + direction.normalized;
        Vector3 targetPos = startPos + direction * range;


        float journey = 0f;
        float travelTime = range / speed;

        // Move forward
        while (journey < 1f)
        {
            journey += Time.deltaTime / travelTime;
            transform.position = Vector3.Lerp(startPos, targetPos, journey);
            yield return null;
        }

        journey = 0f; // Reset journey for return trip

        // Move back
        while (journey < 1f)
        {
            journey += Time.deltaTime / travelTime;
            var returnPosition = playerReference.Transform.position + (targetPos - playerReference.Transform.position).normalized;
            transform.position = Vector3.Lerp(targetPos, returnPosition, journey);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
