using System.Collections;
using UnityEngine;

public class ActivityZoneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ActivityReference activity;
    [SerializeField] private GameObject zoneController;

    [Header("Settings")]
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private Vector3 endScale;

    private void OnEnable()
    {
        activity.OnActivityHasStarted += OnActivityStart;
        activity.OnActivityHasEnded += OnActivityHasEnded;
    }

    private void OnDisable()
    {
        activity.OnActivityHasStarted -= OnActivityStart;
        activity.OnActivityHasEnded -= OnActivityHasEnded;
    }

    private void OnActivityStart()
    {
        // Move player to origin
        transform.position = activity.Origin;

        // Show and animate zone
        zoneController.SetActive(true);
        StartCoroutine(ElasticScale(zoneController.transform, startScale, endScale, scaleDuration));
    }

    private void OnActivityHasEnded()
    {
        // Animate scale down and hide zone
        StartCoroutine(HideZone());
    }

    private IEnumerator HideZone()
    {
        yield return ElasticScale(zoneController.transform, endScale, startScale, scaleDuration);
        zoneController.SetActive(false);
    }

    private IEnumerator ElasticScale(Transform target, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Stable elastic ease-out
            float elasticT = Mathf.Sin(t * Mathf.PI * 0.5f) * (1f + 0.2f * Mathf.Sin(t * Mathf.PI * 3f));

            target.localScale = Vector3.Lerp(from, to, elasticT);
            yield return null;
        }

        target.localScale = to;
    }
}