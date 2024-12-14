using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// todo: break up
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Tutorial tutorial;
    [SerializeField] private Vector3 axis;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private DisplayName displayName;
    [SerializeField] private FadeCanvasGroup title;
    [SerializeField] private FadeCanvasGroup namePrompt;
    [SerializeField] private FadeCanvasGroup mainMenu;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private LocalData localData;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button bossButton;
    [SerializeField] private Transform skyBox;
    [SerializeField] private GameObject egg;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private FungalController fungalPrefab;
    [SerializeField] private Transform fungalSpawnPosition;
    [SerializeField] private Controller controller;

    private GameObject currentFungal;

    private void Awake()
    {
        title.gameObject.SetActive(false);
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

    private void ShowInitialView()
    {
        if (initalViewShown)
        {
            if (fungalInventory.Fungals.Count > 0)
            {
                StartCoroutine(ShowMainMenu());
            }
            else
            {
                StartCoroutine(ShowNamePrompt());
            }
        }
        else
        {
            initalViewShown = true;
            StartCoroutine(ShowTitle());
        }
    }

    private void Update()
    {
        skyBox.Rotate(axis, rotationSpeed * Time.deltaTime);
    }

    private float tranistionDelay = 0.25f;
    private IEnumerator ShowTitle()
    {
        yield return new WaitForSeconds(tranistionDelay);
        yield return title.FadeIn();

        Debug.Log("Waiting for input");

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        yield return title.FadeOut();

        Debug.Log("got for input");

        if (fungalInventory.Fungals.Count > 0)
        {
            StartCoroutine(ShowMainMenu());
        }
        else
        {
            StartCoroutine(ShowNamePrompt());
        }
    }

    private IEnumerator ShowMainMenu()
    {
        yield return new WaitForSeconds(tranistionDelay);
        var fungal = Instantiate(fungalPrefab, fungalSpawnPosition.position, Quaternion.LookRotation(Vector3.back + Vector3.right));
        fungal.Initialize(fungalInventory.Fungals[0], isGrove: false);
        currentFungal = fungal.gameObject;

        // todo: shouldn't need to reference controller
        controller.SetMovement(fungal.Movement);
        yield return mainMenu.FadeIn();
    }

    private IEnumerator ShowNamePrompt()
    {
        yield return new WaitForSeconds(tranistionDelay);
        yield return namePrompt.FadeIn();
    }

    private IEnumerator OnNewGameButtonClicked()
    {
        localData.ResetData();
        if (currentFungal) Destroy(currentFungal);
        yield return mainMenu.FadeOut();
        yield return ShowNamePrompt();
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
        sceneNavigation.NavigateToScene(4);
    }

    private IEnumerator OnSubmitButtonClicked()
    {
        displayName.SetValue(inputField.text);
        yield return namePrompt.FadeOut();

        var randomIndex = Random.Range(0, fungalCollection.Data.Count);
        var randomFungal = fungalCollection.Data[randomIndex];
        var fungal = ScriptableObject.CreateInstance<FungalModel>();
        fungal.Initialize(randomFungal);
        fungalInventory.AddFungal(fungal);


        egg.SetActive(true);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        egg.SetActive(false);


        yield return ShowMainMenu();
    }


}
