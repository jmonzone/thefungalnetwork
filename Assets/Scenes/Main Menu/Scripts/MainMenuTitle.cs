using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTitle : MonoBehaviour
{
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewController titleViewController;
    [SerializeField] private ViewReference titleViewReference;
    [SerializeField] private ViewReference mainMenuViewReference;
    [SerializeField] private ViewReference namePromptViewReference;
    [SerializeField] private ViewReference matchmakingViewReference;
    [SerializeField] private ViewReference partyViewReference;

    [SerializeField] private FadeCanvasGroup tapToContinueText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private FungalInventory fungalInventory;

    private Coroutine fadeInCoroutine;

    private void Awake()
    {
        tapToContinueText.gameObject.SetActive(false);
        titleViewController.OnFadeInComplete += () =>
        {
            StartCoroutine(ShowTitle());
        };
    }

    private void OnEnable()
    {
        sceneNavigation.OnSceneFadeIn += ShowInitialUI;
    }

    private void OnDisable()
    {
        sceneNavigation.OnSceneFadeIn -= ShowInitialUI;
    }

    private void ShowInitialUI()
    {
        var targetUI = titleViewReference;
        if (multiplayer.JoinedLobby != null)
        {
            targetUI = partyViewReference;
            navigation.InitalizeHistory(new List<ViewReference>
            {
                titleViewReference, mainMenuViewReference, matchmakingViewReference
            });
        }
        navigation.Navigate(targetUI);
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
