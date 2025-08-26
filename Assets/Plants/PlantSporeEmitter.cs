using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class PlantSporeEmitter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject sporePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private DJTableReference dJTableReference;

    [Header("Spore Settings")]
    [SerializeField] private float emissionRate = 3f;
    [SerializeField] private float launchHeight = 3f;
    [SerializeField] private float driftDownDuration = 2f;
    [SerializeField] private float landingRadius = 2f;

    [Header("Plant Animation")]
    [SerializeField] private float bounceScale = 1.2f;
    [SerializeField] private float bounceDuration = 0.3f;

    private void Awake()
    {
        navMeshSurface = FindObjectOfType<NavMeshSurface>();
    }

    private void Start()
    {
        StartCoroutine(EmitRoutine());
    }

    private IEnumerator EmitRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(4 * 60 / dJTableReference.BPM);

            StartCoroutine(EmitSpore());
        }
    }

    private IEnumerator EmitSpore()
    {
        // Bounce animation for the plant
        yield return StartCoroutine(BounceAnimation());

        // Create spore
        GameObject spore = Instantiate(sporePrefab, spawnPoint.position, Quaternion.identity);

        // Launch upward
        Vector3 peak = spawnPoint.position + Vector3.up * launchHeight;

        // Find landing spot (not on plant, but near)
        Vector3 landingSpot = FindLandingSpot();

        // Animate spore: up → down like feather
        yield return StartCoroutine(AnimateSpore(spore.transform, peak, landingSpot));

        // Place spore
        spore.transform.position = landingSpot;
    }

    private IEnumerator BounceAnimation()
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * bounceScale;
        float t = 0f;

        // Scale up
        while (t < bounceDuration)
        {
            t += Time.deltaTime;
            float progress = t / bounceDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, Mathf.Sin(progress * Mathf.PI));
            yield return null;
        }

        transform.localScale = startScale;
    }

    private Vector3 FindLandingSpot()
    {
        Vector3 randomDir = Random.insideUnitSphere * landingRadius;
        randomDir.y = 0;
        Vector3 targetPos = transform.position + randomDir;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // fallback: just drop next to plant
        return transform.position + (Vector3.right * 1f);
    }

    private IEnumerator AnimateSpore(Transform spore, Vector3 peak, Vector3 landing)
    {
        Vector3 start = spore.position;
        float t = 0;

        // Go upward
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            spore.position = Vector3.Lerp(start, peak, t / 0.5f);
            yield return null;
        }

        // Drift down with side sway like feather
        t = 0;
        while (t < driftDownDuration)
        {
            t += Time.deltaTime;
            float progress = t / driftDownDuration;

            Vector3 pos = Vector3.Lerp(peak, landing, progress);
            pos.x += Mathf.Sin(progress * Mathf.PI * 2f) * 0.2f; // feather sway
            spore.position = pos;

            yield return null;
        }

        spore.position = landing;
    }
}
