using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuTitle : MonoBehaviour
{
    [SerializeField] private bool debug;

    [SerializeField] private ViewController titleViewController;
    [SerializeField] private Button partyButton;
    [SerializeField] private FadeCanvasGroup partyButtonFade;
    [SerializeField] private ViewReference titleViewReference;

    [SerializeField] private Navigation navigation;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private MultiplayerManager multiplayer;

    [SerializeField] private ViewReference homeView;
    [SerializeField] private ViewReference matchmakingView;
    [SerializeField] private ViewReference partyView;

    [SerializeField] private TextMeshProUGUI versionText;

    private void Awake()
    {
        versionText.text = $"Version {Application.version}";
        titleViewController.OnFadeInComplete += () =>
        {
            StartCoroutine(ShowTitle());
        };

        partyButton.gameObject.SetActive(false);
        partyButton.onClick.AddListener(() =>
        {
            StopAllCoroutines();
            navigation.Navigate(homeView);
        });
    }

    private IEnumerator ShowTitle()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(partyButtonFade.FadeIn());
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
            targetUI = partyView;
            navigation.InitalizeHistory(new List<ViewReference>
            {
                homeView, matchmakingView
            });
        }
        else if (debug)
        {
            targetUI = matchmakingView;
            navigation.InitalizeHistory(new List<ViewReference>
            {
                homeView
            });
        }

        navigation.Navigate(targetUI);
    }
}
