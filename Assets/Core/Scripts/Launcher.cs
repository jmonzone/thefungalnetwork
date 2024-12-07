using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{
    [SerializeField] private Tutorial tutorial;
    [SerializeField] private Vector3 axis;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private DisplayName displayName;
    [SerializeField] private CanvasGroup namePrompt;
    [SerializeField] private CanvasGroup mainMenu;
    [SerializeField] private CanvasGroup screenFade;
    [SerializeField] private LocalData localData;
    [SerializeField] private float transitionDuration = 2f;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        namePrompt.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
        screenFade.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(value => submitButton.interactable = value.Length > 2);

        submitButton.interactable = false;

        submitButton.onClick.AddListener(() =>
        {
            StartCoroutine(OnSubmitButtonClicked());
        });

        newGameButton.onClick.AddListener(() =>
        {
            StartCoroutine(OnNewGameButtonClicked());
        });

        continueButton.onClick.AddListener(() =>
        {
            StartCoroutine(NavigateToScene(2));
        });
    }

    private IEnumerator Start()
    {
        screenFade.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);

        yield return FadeOut(screenFade);

        if (tutorial.IsCompleted)
        {
            yield return ShowMainMenu();
        }
        else
        {
            yield return ShowNamePrompt();
        }
    }

    private void Update()
    {
        mainCamera.transform.Rotate(axis, rotationSpeed * Time.deltaTime);
    }

    private IEnumerator ShowMainMenu()
    {
        yield return new WaitForSeconds(1f);
        yield return FadeIn(mainMenu);
    }

    private IEnumerator ShowNamePrompt()
    {
        yield return new WaitForSeconds(2f);
        yield return FadeIn(namePrompt);
    }

    private IEnumerator OnNewGameButtonClicked()
    {
        localData.ResetData();
        yield return FadeOut(mainMenu);
        yield return ShowNamePrompt();
    }

    private IEnumerator OnSubmitButtonClicked()
    {
        displayName.SetValue(inputField.text);
        yield return FadeOut(namePrompt);
        yield return NavigateToScene(1);
    }

    // todo: centralize scene navigation
    private IEnumerator NavigateToScene(int buildIndex)
    {
        yield return FadeIn(screenFade);
        SceneManager.LoadScene(buildIndex);
    }

    //todo: handle in separate fade component
    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        yield return FadeCanvasGroup(canvasGroup, 0f, 1f);
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        yield return FadeCanvasGroup(canvasGroup, 1f, 0);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha)
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
