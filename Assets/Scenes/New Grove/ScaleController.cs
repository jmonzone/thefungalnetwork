using System.Collections;
using UnityEngine;

public class ScaleController : MonoBehaviour
{
    public float scaleDuration = 0.3f;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public IEnumerator ScaleDown()
    {
        // Animate scale down
        float elapsed = 0f;
        Vector3 startScale = originalScale;
        while (elapsed < scaleDuration)
        {
            float t = elapsed / scaleDuration;
            float easedT = Mathf.SmoothStep(0, 1, t);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator ScaleUp()
    {
        gameObject.SetActive(true);
        yield return null;

        // Respawn and scale up
        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            float t = elapsed / scaleDuration;
            float easedT = Mathf.SmoothStep(0, 1, t);
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }
}
