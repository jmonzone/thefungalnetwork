using System.Collections;
using UnityEngine;

public class MainMenuTitle : MonoBehaviour
{
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewController titleViewController;
    [SerializeField] private ViewReference mainMenuViewReference;
    [SerializeField] private ViewReference namePromptViewReference;
    [SerializeField] private FadeCanvasGroup tapToContinueText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private FungalInventory fungalInventory;

    private Coroutine fadeInCoroutine;

    private void Awake()
    {
        titleViewController.OnViewShowComplete += () =>
        {
            tapToContinueText.gameObject.SetActive(false);
            StartCoroutine(ShowTitle());
        };
    }

    private IEnumerator ShowTitle()
    {
        var elapsedTime = 0f;
        while (true)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > 2 && fadeInCoroutine == null)
            {
                fadeInCoroutine = StartCoroutine(tapToContinueText.FadeIn());
            }

            if (Input.GetMouseButtonDown(0)) break;
            yield return null;
        }

        audioSource.Play();

        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
            fadeInCoroutine = null;
        }

        if (fungalInventory.Fungals.Count > 0)
        {
            navigation.Navigate(mainMenuViewReference);
        }
        else
        {
            navigation.Navigate(namePromptViewReference);
        }
    }
}
