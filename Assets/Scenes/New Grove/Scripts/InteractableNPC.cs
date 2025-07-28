using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableNPC : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private FadeCanvasGroup dialogueView;
    [SerializeField] private TextMeshProUGUI dialogueHeaderText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] [TextArea] private string level1Dialogue;
    [SerializeField] [TextArea] private string level2Dialogue;
    [SerializeField] private Button continueButton;
    [SerializeField] private FadeCanvasGroup touchIndicator;
    [SerializeField] private Animator animator;

    public Transform Transform => transform;

    public event UnityAction OnInteractionStart;
    public event UnityAction OnInteractionComplete;

    private void Awake()
    {
        inventoryController.OnInsightGained += InventoryController_OnInsightGained;

        continueButton.onClick.AddListener(() =>
        {
            StopAllCoroutines();
            StartCoroutine(dialogueView.FadeOut());
        });
    }

    private void InventoryController_OnInsightGained()
    {
        StartCoroutine(touchIndicator.FadeIn());
    }

    public void OnBaseInteraction()
    {
        animator.Play("Jump");

        StopAllCoroutines();

        if (inventoryController.MushroomCount >= 5)
        {
            dialogueHeaderText.text = "Party Frog";
            dialogueText.text = level2Dialogue;
        }
        else
        {
            dialogueHeaderText.text = "a frog?";
            dialogueText.text = level1Dialogue;
        }

        cameraPanController.CenterTargetInView(transform);

        if (touchIndicator.gameObject.activeSelf) StartCoroutine(touchIndicator.FadeOut());
        StartCoroutine(dialogueView.FadeIn());
    }
}
