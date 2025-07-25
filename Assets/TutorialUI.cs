using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private FadeCanvasGroup tutorialTextFade;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private List<string> tutorialStages;

    [SerializeField] private List<ScaleController> interactionIndicators;

    private Camera mainCamera;
    private int tutorialIndex = 0;

    public float requiredMoveDistance = 1.0f; // meters player must move camera to complete

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        var initalUI = GetComponent<InitialUI>();
        initalUI.OnInitialUIShown += InitalUI_OnInitialUIShown;

        tutorialTextFade.gameObject.SetActive(false);

        foreach(var interactionIndicator in interactionIndicators)
        {
            interactionIndicator.gameObject.SetActive(false);
        }
    }

    private void InitalUI_OnInitialUIShown()
    {
        StartCoroutine(ShowTutorialText());
    }

    private IEnumerator ShowTutorialText()
    {
        yield return new WaitForSeconds(2f);

        tutorialText.text = tutorialStages[tutorialIndex];
        yield return tutorialTextFade.FadeIn();

        Vector3 lastPosition = mainCamera.transform.position;
        float totalMoved = 0f;

        yield return new WaitUntil(() =>
        {
            Vector3 currentPosition = mainCamera.transform.position;
            float delta = Vector3.Distance(currentPosition, lastPosition);

            // Only add if movement is significant (to avoid noise)
            if (delta > 0.001f)
            {
                totalMoved += delta;
                lastPosition = currentPosition;
            }

            return totalMoved >= requiredMoveDistance;
        });

        yield return tutorialTextFade.FadeOut();

        yield return new WaitForSeconds(2f);

        tutorialIndex++;
        tutorialText.text = tutorialStages[tutorialIndex];
        yield return tutorialTextFade.FadeIn();

        foreach (var interactionIndicator in interactionIndicators)
        {
            StartCoroutine(interactionIndicator.ScaleUp());
        }
    }
}
