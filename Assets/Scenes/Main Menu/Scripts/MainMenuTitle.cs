using System.Collections;
using TMPro;
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

    [SerializeField] private TextMeshProUGUI versionText;


    private void Awake()
    {
        versionText.text = $"Version {Application.version}";

        partyButton.gameObject.SetActive(false);
        titleViewController.OnFadeInComplete += () =>
        {
            StartCoroutine(ShowTitle());
        };
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
        StartCoroutine(NavigateToInitialUI());
    }

    private IEnumerator NavigateToInitialUI()
    {
        yield return new WaitForSeconds(3f);
        navigation.Navigate(titleViewReference);

    }
}
