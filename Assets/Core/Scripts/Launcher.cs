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
    [SerializeField] private LocalData localData;
    [SerializeField] private float transitionDuration = 2f;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;

    private Camera mainCamera;

    private enum State
    {
        MENU,
        NAME_PROMPT
    }

    private void Start()
    {
        mainCamera = Camera.main;

        namePrompt.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(value => submitButton.interactable = value.Length > 2);

        submitButton.interactable = false;

        submitButton.onClick.AddListener(() =>
        {
            StartCoroutine(OnSubmitButtonClicked());
        });

        newGameButton.onClick.AddListener(() =>
        {
            StartCoroutine(OnNewGaneButtonClicked());
        });

        continueButton.onClick.AddListener(() => SceneManager.LoadScene(2));

        if (tutorial.IsCompleted)
        {
            SetState(State.MENU);
        }
        else
        {
            SetState(State.NAME_PROMPT);
        }
    }

    private IEnumerator OnNewGaneButtonClicked()
    {
        localData.ResetData();
        yield return FadeCanvasGroup(mainMenu, 1f, 0f, transitionDuration);
        SetState(State.NAME_PROMPT);
    }

    private IEnumerator OnSubmitButtonClicked()
    {
        displayName.SetValue(inputField.text);
        yield return FadeCanvasGroup(namePrompt, 1f, 0f, transitionDuration);
        SceneManager.LoadScene(1);
    }

    private void SetState(State state)
    {
        switch (state)
        {
            case State.MENU:
                FadeInCanvasGroup(mainMenu);
                break;
            case State.NAME_PROMPT:
                FadeInCanvasGroup(namePrompt);
                break;
        }
    }

    private void FadeInCanvasGroup(CanvasGroup canvasGroup)
    {
        // Start the fade-in coroutine
        StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, transitionDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        canvasGroup.alpha = startAlpha;
        canvasGroup.blocksRaycasts = startAlpha > 0; // Disable interaction if starting from invisible
        canvasGroup.gameObject.SetActive(true);

        float elapsedTime = 0f;

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

    private void Update()
    {
        mainCamera.transform.Rotate(axis, rotationSpeed * Time.deltaTime);
    }
}
