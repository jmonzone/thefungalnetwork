using System.Collections;
using UnityEngine;

public class FishingRodProjectile : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float speed = 7.5f;
    [SerializeField] private float range = 5f;
    [SerializeField] private float detectionRadius = 1f;

    public Pufferfish Pufferfish { get; private set; }

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

    private bool DetectPufferfishHit()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f); // Small detection radius

        foreach (Collider hit in hits)
        {
            Pufferfish = hit.GetComponentInParent<Pufferfish>();
            if (Pufferfish != null)
            {
                Pufferfish.Catch(transform);
                return true; // Collision detected
            }
        }
        return false;
    }

    private IEnumerator MoveProjectile(Vector3 direction)
    {
        Vector3 startPos = playerReference.Transform.position + direction.normalized;
        Vector3 targetPos = startPos + direction.normalized * range;

        transform.position = startPos;

        // Move forward
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            if (DetectPufferfishHit()) break;
            yield return null;
        }

        // Move back to the player
        while (Vector3.Distance(transform.position, playerReference.Transform.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerReference.Transform.position, speed * Time.deltaTime);
            yield return null;
        }

        if (Pufferfish) Pufferfish.PickUp();
        gameObject.SetActive(false);
    }
}
