using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// todo: break up
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Tutorial tutorial;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private FadeCanvasGroup namePrompt;
    [SerializeField] private FadeCanvasGroup mainMenu;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private LocalData localData;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button bossButton;
    [SerializeField] private GameObject egg;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private FungalController fungalPrefab;
    [SerializeField] private Transform fungalSpawnPosition;
    [SerializeField] private Controller controller;
    [SerializeField] private MainMenuUIState initialState;
    [SerializeField] private ViewReference matchmaking;
    [SerializeField] private Navigation navigation;

    private enum MainMenuUIState
    {
        TITLE,
        MENU,
        MATCHMAKING,
        NAME_PROMPT
    }

    private GameObject currentFungal;

    private void Awake()
    {
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
            StartCoroutine(OnNewGameButtonClicked());
        });

        continueButton.onClick.AddListener(() =>
        {
            StartCoroutine(OnContinueButtonClicked());
        });

        bossButton.onClick.AddListener(() =>
        {
            StartCoroutine(OnBossButtonClicked());
        });

        //title.OnComplete += () => GoToFirstScene();
    }

    private void OnEnable()
    {
        sceneNavigation.OnSceneFadeIn += ShowInitialView;
    }

    private void OnDisable()
    {
        sceneNavigation.OnSceneFadeIn -= ShowInitialView;
    }

    private static bool initalViewShown = false;

    private void GoToFirstScene()
    {
        if (fungalInventory.Fungals.Count > 0)
        {
            StartCoroutine(SetUIState(MainMenuUIState.MENU));
        }
        else
        {
            StartCoroutine(SetUIState(MainMenuUIState.NAME_PROMPT));
        }
    }

    private void ShowInitialView()
    {
        //if (initalViewShown)
        //{
        //    GoToFirstScene();
        //}
        //else
        //{
        //    initalViewShown = true;

        //    if (Application.isEditor)
        //    {
        //        StartCoroutine(SetUIState(initialState));
        //    }
        //    else
        //    {
        //        StartCoroutine(SetUIState(MainMenuUIState.TITLE));
        //    }
        //}
    }

    private const float MENU_TRANSITION_DELAY = 0.25f;

    private IEnumerator SetUIState(MainMenuUIState state)
    {
        yield return new WaitForSeconds(MENU_TRANSITION_DELAY);

        switch (state)
        {
            case MainMenuUIState.TITLE:
                //yield return title.ShowTitle();
                break;
            case MainMenuUIState.MENU:
                var fungal = Instantiate(fungalPrefab, fungalSpawnPosition.position, Quaternion.LookRotation(Vector3.back + Vector3.right));
                fungal.Initialize(fungalInventory.Fungals[0], isGrove: false);
                currentFungal = fungal.gameObject;

                // todo: shouldn't need to reference controller
                controller.SetMovement(fungal.Movement);
                yield return mainMenu.FadeIn();
                break;
            case MainMenuUIState.MATCHMAKING:
                navigation.Navigate(matchmaking);
                break;
            case MainMenuUIState.NAME_PROMPT:
                yield return namePrompt.FadeIn();
                break;
        }
    }

    private IEnumerator OnNewGameButtonClicked()
    {
        localData.ResetData();
        if (currentFungal) Destroy(currentFungal);
        yield return mainMenu.FadeOut();
        GoToFirstScene();
    }

    private IEnumerator OnContinueButtonClicked()
    {
        yield return mainMenu.FadeOut();

        if (tutorial.IsCompleted)
        {
            sceneNavigation.NavigateToScene(2);
        }
        else
        {
            sceneNavigation.NavigateToScene(1);
        }
    }

    private IEnumerator OnBossButtonClicked()
    {
        yield return mainMenu.FadeOut();
        StartCoroutine(SetUIState(MainMenuUIState.MATCHMAKING));
    }

    private IEnumerator OnSubmitButtonClicked()
    {
        yield return namePrompt.FadeOut();

        var randomIndex = Random.Range(0, fungalCollection.Fungals.Count);
        var randomFungal = fungalCollection.Fungals[randomIndex];
        var fungal = ScriptableObject.CreateInstance<FungalModel>();
        fungal.Initialize(randomFungal);
        fungalInventory.AddFungal(fungal);


        egg.SetActive(true);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        egg.SetActive(false);

        yield return SetUIState(MainMenuUIState.MENU);
    }


}
