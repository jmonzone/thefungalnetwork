using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class PlantSporeEmitter : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private SporeController sporePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private DJTableReference dJTableReference;

    [Header("Spore Settings")]
    [SerializeField] private float emissionRate = 3f;
    [SerializeField] private float launchHeight = 2f;
    [SerializeField] private float landingRadius = 2f;

    [Header("Plant Animation")]
    [SerializeField] private float bounceScale = 1.2f;
    [SerializeField] private float bounceDuration = 0.3f;

    Transform IInteractable.Transform => transform;

    private Vector3 startScale;

    private void Awake()
    {
        startScale = transform.localScale;
    }

    void IInteractable.OnSelect()
    {
        StopAllCoroutines();
        StartCoroutine(BounceAnimation());

        EmitSpore();
    }

    private void EmitSpore()
    {
        // Create spore
        SporeController spore = Instantiate(sporePrefab, spawnPoint.position, Quaternion.identity);

        // Launch upward
        Vector3 peak = spawnPoint.position + Vector3.up * launchHeight;

        // Find landing spot (not on plant, but near)
        Vector3 landingSpot = FindLandingSpot();

        spore.LaunchSpore(peak, landingSpot);
    }

    private IEnumerator BounceAnimation()
    {
        transform.localScale = startScale;

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
        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        randomDirection.z = randomDirection.y;
        randomDirection.y = 0;
        randomDirection *= Random.Range(1.25f, 1.75f);

        Vector3 targetPos = transform.position + randomDirection;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        Debug.Log("fallback");
        // fallback: just drop next to plant
        return transform.position + (Vector3.right * 1f);
    }
}
