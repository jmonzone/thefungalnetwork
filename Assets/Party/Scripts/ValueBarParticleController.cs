using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ValueBarParticleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform targetUI;
    [SerializeField] private GameObject particlePrefab;

    [Header("Settings")]
    [SerializeField] private Color startColor;
    [SerializeField] private Color targetColor;
    [SerializeField] private float mintravelTime = 0.75f;
    [SerializeField] private float maxtravelTime = 1.25f;
    [SerializeField] private AnimationCurve arcCurve;

    private Canvas canvas;
    private List<GameObject> activeParticles = new List<GameObject>();

    public event UnityAction OnParticleReached;
    public event UnityAction OnAllParticleReached;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    private void OnDisable()
    {
        foreach(var particle in activeParticles)
        {
            Destroy(particle);
        }

        activeParticles = new List<GameObject>();
    }

    public void SetStartColor(Color color)
    {
        startColor = color;
    }

    public void SetTargetColor(Color color)
    {
        targetColor = color;
    }

    public void BurstFromWorld(int count, Vector3 screenPos)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject p = Instantiate(particlePrefab, canvas.transform);
            activeParticles.Add(p);
            StartCoroutine(AnimateParticle(p, screenPos));
        }
    }

    private IEnumerator AnimateParticle(GameObject particle, Vector3 screenPos)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 localStart
        );

        RectTransform rect = particle.GetComponent<RectTransform>();
        rect.SetParent(canvasRect, false);
        rect.anchoredPosition = localStart;

        var image = particle.GetComponentInChildren<Image>();
        image.color = startColor;

        float t = 0;
        float randomOffset = Random.Range(-100f, 100f);

        var travelTime = Random.Range(mintravelTime, maxtravelTime);


        while (t < 1f)
        {
            // End pos in canvas space
            Vector3 screenEnd = RectTransformUtility.WorldToScreenPoint(null, targetUI.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenEnd, null, out Vector2 localEnd
            );

            t += Time.deltaTime / travelTime;

            // Arc path
            Vector2 pos = Vector2.Lerp(localStart, localEnd, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * randomOffset * (1 - t);
            pos.x += arcCurve.Evaluate(t / travelTime) * randomOffset * (1 - t);

            targetColor.a = 1 - t;
            var lerpColor = Color.Lerp(startColor, targetColor, t);
            image.color = lerpColor;

            rect.anchoredPosition = pos;
            yield return null;
        }

        activeParticles.Remove(particle);
        Destroy(particle);
        OnParticleReached?.Invoke();

        if (activeParticles.Count == 0) OnAllParticleReached?.Invoke();
    }
}
