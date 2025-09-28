using System.Collections;
using UnityEngine;

public class DancefloorController : MonoBehaviour
{
    [SerializeField] private DancefloorReference dancefloorReference;
    [SerializeField] private PlayerReference playerReference;

    [SerializeField] private ZoneController zoneController;

    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private float targetScale = 1f;

    private void OnDancefloorStart()
    {
        // Move player to origin
        transform.position = dancefloorReference.Origin;

        // Show and animate zone
        zoneController.gameObject.SetActive(true);
        StartCoroutine(ElasticScale(zoneController.transform, Vector3.zero, Vector3.one * targetScale, scaleDuration));
    }

    private void OnDancefloorEnd()
    {
        // Animate scale down and hide zone
        StartCoroutine(HideZone());
    }

    private IEnumerator HideZone()
    {
        yield return ElasticScale(zoneController.transform, zoneController.transform.localScale, Vector3.zero, scaleDuration);
        zoneController.gameObject.SetActive(false);
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

    private void Awake()
    {
        //zoneController.OnPlayerEnterZone += musicVideoReference.EnterDancefloor;
    }

    private void OnEnable()
    {
        dancefloorReference.OnDancefloorStart += OnDancefloorStart;
        dancefloorReference.OnDancefloorExit += OnDancefloorEnd;
    }

    private void OnDisable()
    {
        dancefloorReference.OnDancefloorStart -= OnDancefloorStart;
        dancefloorReference.OnDancefloorExit -= OnDancefloorEnd;
    }

   
}
