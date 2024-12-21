using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuTitle : MonoBehaviour
{
    [SerializeField] private ViewController titleViewController;
    [SerializeField] private Button partyButton;
    [SerializeField] private FadeCanvasGroup partyButtonFade;
    [SerializeField] private ViewReference titleViewReference;

    [SerializeField] private Navigation navigation;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private MultiplayerManager multiplayer;

    [SerializeField] private ViewReference matchmakingViewReference;
    [SerializeField] private ViewReference partyViewReference;

    private void Awake()
    {
        titleViewController.OnFadeInComplete += () =>
        {
            StartCoroutine(ShowTitle());
        };

        partyButton.gameObject.SetActive(false);
        partyButton.onClick.AddListener(() =>
        {
            StopAllCoroutines();
            navigation.Navigate(matchmakingViewReference);
        });
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
                titleViewReference, matchmakingViewReference
            });
        }
        navigation.Navigate(targetUI);
    }

    private IEnumerator ShowTitle()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(partyButtonFade.FadeIn());
    }
}
