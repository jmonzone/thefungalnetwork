using System.Collections;
using UnityEngine;

public class TouchIndicatorWorld : MonoBehaviour
{
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private GameObject indicatorRender;
    [SerializeField] private SpriteRenderer ringRenderer;
    [SerializeField] private SpriteRenderer circleRenderer;

    private Transform target;

    private void Awake()
    {
        interactionController.OnEntitySelected += entity =>
        {
            target = entity;

            if (!entity)
            {
                HideIndicator();
            }
            else
            {
                ShowIndicator(entity.position, true);
            }
        };

        interactionController.OnGroundSelected += position =>
        {
            target = null;
            ShowIndicator(position, true);
        };

        dialogue.OnDialogueComplete += () =>
        {
            target = null;
            HideIndicator();
        };
    }

    private void Update()
    {
        if (target) indicatorRender.transform.position = target.position;
    }

    private void ShowIndicator(Vector3 position, bool dissipate)
    {
        indicatorRender.transform.position = position;
        indicatorRender.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(TouchAnimation(dissipate));
    }

    private void HideIndicator()
    {
        StopAllCoroutines();
        StartCoroutine(DissipateRoutine());
    }

    // Ring scales
    private float ringFocus = 1.0f;
    private float circleFocus = 0.9f;

    private IEnumerator TouchAnimation(bool dissipate)
    {
        float honeDuration = 0.3f;

        float ringStart = 1.6f;
        float circleStart = 1.2f;

        float elapsed = 0f;

        // Reset alphas and scales
        SetAlpha(1f);
        ringRenderer.transform.localScale = Vector3.one * ringStart;
        circleRenderer.transform.localScale = Vector3.one * circleStart;
        indicatorRender.SetActive(true);

        // Hone in
        while (elapsed < honeDuration)
        {
            float t = elapsed / honeDuration;
            float eased = Mathf.SmoothStep(0f, 1f, t);

            ringRenderer.transform.localScale = Vector3.one * Mathf.Lerp(ringStart, ringFocus, eased);
            circleRenderer.transform.localScale = Vector3.one * Mathf.Lerp(circleStart, circleFocus, eased);

            elapsed += Time.deltaTime;
            yield return null;
        }

        ringRenderer.transform.localScale = Vector3.one * ringFocus;
        circleRenderer.transform.localScale = Vector3.one * circleFocus;

        if (dissipate) yield return DissipateRoutine();

    
    }

    private IEnumerator DissipateRoutine()
    {
        float fadeDuration = 0.4f;
        float ringEnd = 1.3f;
        float circleEnd = 1.0f;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;

            float alpha = Mathf.Lerp(1f, 0f, t);
            SetAlpha(alpha);

            ringRenderer.transform.localScale = Vector3.one * Mathf.Lerp(ringFocus, ringEnd, t);
            circleRenderer.transform.localScale = Vector3.one * Mathf.Lerp(circleFocus, circleEnd, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        indicatorRender.SetActive(false);
    }

    private void SetAlpha(float a)
    {
        if (ringRenderer != null)
        {
            Color color = ringRenderer.color;
            color.a = a;
            ringRenderer.color = color;
        }

        if (circleRenderer != null)
        {
            Color color = circleRenderer.color;
            color.a = a;
            circleRenderer.color = color;
        }
    }
}
