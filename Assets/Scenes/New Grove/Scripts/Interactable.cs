using TMPro;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private string id;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Vector2 spritePosition;
    [SerializeField] private Vector2 spriteSize;
    [SerializeField] private int level;


    public string Id => id;
    public Sprite Sprite => sprite;
    public Vector2 SpritePosition => spritePosition;
    public Vector2 SpriteSize => spriteSize;

    public int Level => level;
    public Transform Transform => transform;

    [SerializeField] private InteractionUI interactionUI;
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private TextMeshProUGUI dialogueHeaderText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private FadeCanvasGroup touchIndicator;

    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference interactionView;

    [SerializeField] [TextArea] private string level1Dialogue;
    [SerializeField] [TextArea] private string level2Dialogue;


    private void Awake()
    {
        interactionUI.OnButtonUnlockable += InteractionUI_OnButtonUnlockable;
    }

    public void OnBaseInteraction()
    {
        StopAllCoroutines();

        if (level == 0)
        {
            dialogueHeaderText.text = "???";
            dialogueText.text = level1Dialogue;
        }
        else
        {
            dialogueHeaderText.text = name;
            dialogueText.text = level2Dialogue;
        }

        cameraPanController.CenterTargetInView(transform);

        if (touchIndicator.gameObject.activeSelf) StartCoroutine(touchIndicator.FadeOut());
        if (navigation.CurrentView != interactionView)
        {
            navigation.Navigate(interactionView);
        }
    }


    private void InteractionUI_OnButtonUnlockable()
    {
        if (!touchIndicator.IsVisible) StartCoroutine(touchIndicator.FadeIn());
    }
}
