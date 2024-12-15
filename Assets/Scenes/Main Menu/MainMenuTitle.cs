using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MainMenuTitle : MonoBehaviour
{
    [SerializeField] private FadeCanvasGroup canvasGroup;
    [SerializeField] private FadeCanvasGroup tapToContinueText;
    [SerializeField] private AudioSource audioSource;

    public event UnityAction OnComplete;

    private void Awake()
    {
        canvasGroup.gameObject.SetActive(false);
        tapToContinueText.gameObject.SetActive(false);
    }

    public IEnumerator ShowTitle()
    {
        yield return canvasGroup.FadeIn();

        var elapsedTime = 0f;
        var fadingIn = false;
        while (true)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > 2 && !fadingIn)
            {
                fadingIn = true;
                StartCoroutine(tapToContinueText.FadeIn());
            }
            if (Input.GetMouseButtonDown(0)) break;
            yield return null;
        }

        audioSource.Play();

        StopAllCoroutines();
        yield return canvasGroup.FadeOut();
        yield return tapToContinueText.FadeOut();
        OnComplete?.Invoke();
    }
}
