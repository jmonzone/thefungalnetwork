using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ValueParticleController : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private Image image;
    [SerializeField] private float mintravelTime = 0.75f;
    [SerializeField] private float maxtravelTime = 1.25f;
    [SerializeField] private AnimationCurve arcCurve;

    public Color startColor;
    public Color targetColor;

    private Canvas canvas;
    private RectTransform targetUI;

    public void Initialize(Vector3 screenPos, RectTransform targetUI, UnityAction onComplete)
    {
        image.color = startColor;

        canvas = GetComponentInParent<Canvas>();
        this.targetUI = targetUI;
        StartCoroutine(AnimateParticle(screenPos, onComplete));
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

    private IEnumerator AnimateParticle(Vector3 startPos, UnityAction onComplete)
    {
        RectTransform parentRect = transform.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, startPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 localStart
        );

        rect.anchoredPosition = localStart;

        float t = 0;
        float randomOffset = Random.Range(-100f, 100f);

        var travelTime = Random.Range(mintravelTime, maxtravelTime);


        while (t < 1f)
        {
            // End pos in canvas space
            Vector3 screenEnd = RectTransformUtility.WorldToScreenPoint(null, targetUI.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, screenEnd, null, out Vector2 localEnd
            );

            t += Time.deltaTime / travelTime;

            // Arc path
            Vector2 pos = Vector2.Lerp(localStart, localEnd, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * randomOffset * (1 - t);
            pos.x += arcCurve.Evaluate(t / travelTime) * randomOffset * (1 - t);

            //targetColor.a = 1 - t;
            var lerpColor = Color.Lerp(startColor, targetColor, t);
            image.color = lerpColor;

            rect.anchoredPosition = pos;
            yield return null;
        }

        onComplete?.Invoke();

    }
}
