using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class FadeCanvasGroup : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public IEnumerator FadeIn(float duration = 1f) => Fade(0f, 1f, duration);

    public IEnumerator FadeOut(float duration = 1f, UnityAction onComplete = null) => Fade(1f, 0, duration, onComplete);

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
            Debug.Log(canvasGroup.alpha);
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
