using System.Collections;
using UnityEngine;

public class FadeCanvasGroup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private float transitionDuration = 2f;

    public IEnumerator FadeIn() => Fade(0f, 1f);

    public IEnumerator FadeOut() => Fade(1f, 0);

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        canvasGroup.alpha = startAlpha;
        canvasGroup.blocksRaycasts = startAlpha > 0; // Disable interaction if starting from invisible
        canvasGroup.gameObject.SetActive(true);

        float elapsedTime = 0f;
        var duration = transitionDuration;

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
    }
}
