using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class DialoguePageUI : MonoBehaviour
{
    [SerializeField] protected DialogueReference dialogue;
    [SerializeField] protected FadeCanvasGroup fadeCanvasGroup;

    public event UnityAction OnClose;

    protected virtual void Awake()
    {

    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    public virtual void Show()
    {
        StartCoroutine(fadeCanvasGroup.FadeIn());
    }

    public virtual void Hide()
    {
        fadeCanvasGroup.gameObject.SetActive(false);
    }

    protected void InvokeClose()
    {
        OnClose?.Invoke();
    }
}

public class DialogueUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueReference dialogue;

    [Header("UI Components")]
    [SerializeField] private SpeakerUI speakerUI;
    [SerializeField] private DialogueActionsUI actionPage;
    [SerializeField] private DialogueChatUI chatPage;
    [SerializeField] private DialogueFriendshipUI friendshipPage;

    [SerializeField] private TarotCardUI tarotCard;
    [SerializeField] private Button closeButton;

    private DialoguePageUI currentPage;

    private void Awake()
    {
        currentPage = actionPage;
        chatPage.OnClose += () => ShowPage(DialoguePage.FRIENDSHIP);

        closeButton.onClick.AddListener(dialogue.CompleteDialogue);
    }

    private void OnEnable()
    {
        dialogue.OnInteractionStart += StartInteraction;
        dialogue.OnDialogueStart += StartDialogue;
        dialogue.OnGiveComplete += StartDialogue;
    }

    private void OnDisable()
    {
        dialogue.OnInteractionStart -= StartInteraction;
        dialogue.OnDialogueStart -= StartDialogue;
        dialogue.OnGiveComplete -= StartDialogue;
    }

    private void StartInteraction()
    {
        ShowPage(DialoguePage.ACTION);
    }

    private void StartDialogue()
    {
        ShowPage(DialoguePage.CHAT);
    }

    private enum DialoguePage
    {
        ACTION,
        CHAT,
        FRIENDSHIP
    }

    private void ShowPage(DialoguePage page)
    {
        speakerUI.SetSpeaker(dialogue.Unit.Instance);

        currentPage.Hide();

        currentPage = page switch
        {
            DialoguePage.ACTION => actionPage,
            DialoguePage.CHAT => chatPage,
            DialoguePage.FRIENDSHIP => friendshipPage,
            _ => actionPage,
        };

        currentPage.Show();
    }
}
