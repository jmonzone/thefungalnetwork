﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class FadeCanvasGroup : MonoBehaviour
{
    private float duration = 0.5f;
    private CanvasGroup canvasGroup;

    public bool IsVisible
    {
        get
        {
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
            return canvasGroup.alpha > 0;
        }
    }

    public IEnumerator FadeIn() => Fade(0f, 1f, duration);
    public IEnumerator FadeIn(float duration) => Fade(0f, 1f, duration);

    public IEnumerator FadeOut() => Fade(1f, 0, duration);
    public IEnumerator FadeOut(float duration, UnityAction onComplete = null) => Fade(1f, 0, duration, onComplete);

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration = 1f, UnityAction onComplete = null)
    {
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = startAlpha;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.gameObject.SetActive(true);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (endAlpha == 0)
        {
            // Fully faded out: make the UI non-interactive and optionally hide the object
            canvasGroup.blocksRaycasts = false;
            canvasGroup.gameObject.SetActive(false);
        }
        else
        {
            // Fully visible: enable interaction
            canvasGroup.blocksRaycasts = true;
        }

        onComplete?.Invoke();
    }
}
