using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    [SerializeField] private MultiplayerReference multiplayer;

    [SerializeField] private ViewReference homeView;
    [SerializeField] private ViewReference matchmakingView;
    [SerializeField] private ViewReference partyView;

    [SerializeField] private TextMeshProUGUI versionText;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

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
            StartCoroutine(GoToIntro());
        });
    }

    private IEnumerator GoToIntro()
    {
        var dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();

        while (dolly.m_PathPosition < 1f)
        {
            dolly.m_PathPosition += Time.deltaTime * 0.5f;
            yield return null;
        }

        sceneNavigation.NavigateToScene(3);
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

        StartCoroutine(NavigateToInitialUI(targetUI));
    }

    private IEnumerator NavigateToInitialUI(ViewReference targetUI)
    {
        yield return new WaitForSeconds(2f);
        navigation.Navigate(targetUI);

    }
}
