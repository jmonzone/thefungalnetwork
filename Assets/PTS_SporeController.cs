using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PTS_SporeController : MonoBehaviour
{
    [SerializeField] private DJTableReference djReference;

    public void Pass(PTS_Unit target, UnityAction onComplete)
    {
        StartCoroutine(PassRoutine(target, onComplete));
    }

    private IEnumerator PassRoutine(PTS_Unit target, UnityAction onComplete)
    {
        float elapsed = 0f;

        // Optional: add an arc height for a nicer toss
        float arcHeight = 1f;

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
}
