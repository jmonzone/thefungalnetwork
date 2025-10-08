using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PTS_SporeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private FresnelMaterialController fresnelMaterial;

    [Header("Animation Settings")]
    [SerializeField] private float arcHeight = 0.5f;
    [SerializeField] private float popDuration = 0.25f;
    [SerializeField] private AnimationCurve popScaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private void Awake()
    {
        if (fresnelMaterial == null) fresnelMaterial = GetComponent<FresnelMaterialController>();
    }

    public void LightSpore()
    {
        fresnelMaterial.Pulse();
    }

    public void Pass(PTS_Unit target, UnityAction onComplete)
    {
        StartCoroutine(PassRoutine(target, onComplete));
    }

    private IEnumerator PassRoutine(PTS_Unit target, UnityAction onComplete)
    {
        float elapsed = 0f;
        var start = transform.position;
        var duration = djReference.BeatDuration * 2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Lerp position
            Vector3 horizontal = Vector3.Lerp(start, target.SporePosition, t);

            // Add simple vertical arc (parabola)
            float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
            transform.position = horizontal + Vector3.up * height;

            yield return null;
        }

        onComplete?.Invoke();
    }

    /// <summary>
    /// Plays a quick "pop" animation before destroying the spore.
    /// </summary>
    public void PopAndDestroy(UnityAction onComplete = null)
    {
        StartCoroutine(PopAndDestroyRoutine(onComplete));
    }

    private IEnumerator PopAndDestroyRoutine(UnityAction onComplete)
    {
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;

        // Optional: emit light pulse before disappearing
        fresnelMaterial?.Pulse();

        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;

            // Animate scale shrinking using curve
            float scale = popScaleCurve.Evaluate(t);
            transform.localScale = originalScale * scale;

            yield return null;
        }

        onComplete?.Invoke();
        Destroy(gameObject);
    }
}
