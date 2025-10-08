using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PortalController : MonoBehaviour
{
    [Header("Timings")]
    [SerializeField] private float openDuration = 1f;
    [SerializeField] private float stayDuration = 1.5f;
    [SerializeField] private float closeDuration = 0.75f;

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Events")]
    public UnityEvent OnOpened;
    public UnityEvent OnClosed;

    private Vector3 initialScale;

    private void Awake()
    {
        initialScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        StartCoroutine(PortalRoutine());
    }

    private IEnumerator PortalRoutine()
    {
        // Open phase
        yield return AnimateScale(Vector3.zero, initialScale, openDuration);
        OnOpened?.Invoke();

        // Stay phase
        yield return new WaitForSeconds(stayDuration);

        // Close phase
        yield return AnimateScale(initialScale, Vector3.zero, closeDuration);
        OnClosed?.Invoke();

        Destroy(gameObject);
    }

    private IEnumerator AnimateScale(Vector3 from, Vector3 to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = scaleCurve.Evaluate(time / duration);
            transform.localScale = Vector3.LerpUnclamped(from, to, t);
            yield return null;
        }
    }
}
