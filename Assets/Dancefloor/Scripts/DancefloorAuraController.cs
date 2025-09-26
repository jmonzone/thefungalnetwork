using UnityEngine;
using UnityEngine.Events;

public class DancefloorAuraController : MonoBehaviour
{
    [Header("Ring Pulse Settings")]
    public GameObject ringPrefab;
    public float ringDuration = 0.4f;
    public float ringMaxScale = 2f;

    private bool isActive = false;

    public event UnityAction OnHit;

    // ---------------------------
    // Public interface for manager
    // ---------------------------
    public void Activate() => isActive = true;
    public void Deactivate() => isActive = false;

    public void Hit()
    {
        if (!isActive) return;

        SpawnRingPulse();
        OnHit?.Invoke();
    }


    private void SpawnRingPulse()
    {
        if (!ringPrefab) return;

        GameObject ring = Instantiate(ringPrefab, transform);
        RectTransform ringRect = ring.GetComponent<RectTransform>();
        ringRect.localScale = Vector3.one;

        StartCoroutine(AnimateRing(ringRect));
    }

    private System.Collections.IEnumerator AnimateRing(RectTransform ring)
    {
        float elapsed = 0f;
        CanvasGroup cg = ring.GetComponent<CanvasGroup>();
        if (cg == null) cg = ring.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        while (elapsed < ringDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / ringDuration;

            ring.localScale = Vector3.one * Mathf.Lerp(1f, ringMaxScale, t);
            cg.alpha = 1f - t;

            yield return null;
        }

        Destroy(ring.gameObject);
    }
}
