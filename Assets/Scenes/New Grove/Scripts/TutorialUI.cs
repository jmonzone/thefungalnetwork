using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum TutorialStage
{
    Movement,
    InteractionPrompt,
    // Add more stages here
}
public class TutorialUI : MonoBehaviour
{
    [SerializeField] private FadeCanvasGroup tutorialTextFade;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private List<string> tutorialStages;

    [SerializeField] private bool showText;

    private List<InteractableMushroom> mushrooms;

    private Camera mainCamera;

    [Header("Debug Settings")]
    public TutorialStage startStage = TutorialStage.Movement;

    private TutorialStage currentStage;
    public float requiredMoveDistance = 1.0f; // meters player must move camera to complete

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        var initalUI = GetComponent<InitialUI>();
        initalUI.OnInitialUIShown += InitalUI_OnInitialUIShown;

        tutorialTextFade.gameObject.SetActive(false);

        mushrooms = FindObjectsOfType<InteractableMushroom>(true).ToList();
        foreach(var mushroom in mushrooms)
        {
            mushroom.gameObject.SetActive(false);
        }
    }

    private void InitalUI_OnInitialUIShown()
    {
        StartCoroutine(ShowTutorial(startStage));
    }

    private IEnumerator ShowTutorial(TutorialStage tutorialStage)
    {
        currentStage = tutorialStage;

        yield return new WaitForSeconds(2f);

        if (currentStage <= TutorialStage.Movement)
            yield return Stage_Movement();

        yield return new WaitForSeconds(2f);

        if (currentStage <= TutorialStage.InteractionPrompt)
            yield return Stage_InteractionPrompt();
    }

    private IEnumerator Stage_Movement()
    {
        if (showText)
        {
            tutorialText.text = GetTextForStage(TutorialStage.Movement);
            yield return tutorialTextFade.FadeIn();
        }

        Vector3 lastPosition = mainCamera.transform.position;
        float totalMoved = 0f;

        yield return new WaitUntil(() =>
        {
            Vector3 currentPosition = mainCamera.transform.position;
            float delta = Vector3.Distance(currentPosition, lastPosition);
            if (delta > 0.001f)
            {
                totalMoved += delta;
                lastPosition = currentPosition;
            }
            return totalMoved >= requiredMoveDistance;
        });

        if (showText)
        {
            yield return tutorialTextFade.FadeOut();
        }
    }

    private IEnumerator Stage_InteractionPrompt()
    {
        if (showText)
        {
            tutorialText.text = GetTextForStage(TutorialStage.InteractionPrompt);
            yield return tutorialTextFade.FadeIn();
        }

        foreach (var mushroom in mushrooms)
        {
            StartCoroutine(mushroom.scaleController.ScaleUp());
            yield return new WaitForSeconds(0.5f);
        }
    }

    private string GetTextForStage(TutorialStage stage)
    {
        int index = (int)stage;
        if (index >= 0 && index < tutorialStages.Count)
            return tutorialStages[index];
        return "";
    }
}
