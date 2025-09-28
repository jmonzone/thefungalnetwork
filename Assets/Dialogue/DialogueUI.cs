using UnityEngine;
using UnityEngine.UI;

public enum DialoguePage
{
    ACTION,
    CHAT,
    GLYPH,
    FRIENDSHIP
}


public class DialogueUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private UnitListReference unitListReference;

    [Header("UI Components")]
    [SerializeField] private SpeakerUI speakerUI;
    [SerializeField] private DialogueActionsUI actionPage;
    [SerializeField] private DialogueChatUI chatPage;
    [SerializeField] private DialogueGlyphUI glyphPage;
    [SerializeField] private DialogueFriendshipUI friendshipPage;

    [SerializeField] private TarotCardUI tarotCard;
    [SerializeField] private Button closeButton;

    private DialoguePageUI currentPage;

    private void Awake()
    {
        currentPage = actionPage;

        actionPage.Hide();
        chatPage.Hide();
        glyphPage.Hide();
        friendshipPage.Hide();

        actionPage.OnClose += CompleteDialogue;
        chatPage.OnClose += CompleteDialogue;
        glyphPage.OnClose += CompleteDialogue;
        friendshipPage.OnClose += CompleteDialogue;
        closeButton.onClick.AddListener(CompleteDialogue);
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
        if (dialogue.Unit is TreeController)
        {
            ShowPage(DialoguePage.GLYPH);
        }
        else
        {
            ShowPage(DialoguePage.ACTION);
        }
    }

    private void StartDialogue()
    {
        ShowPage(DialoguePage.CHAT);
    }

    private void CompleteDialogue()
    {
        dialogue.CompleteDialogue();
    }

    private void ShowPage(DialoguePage page)
    {
        speakerUI.SetSpeaker(dialogue.Unit.Instance);

        currentPage.Hide();

        currentPage = page switch
        {
            DialoguePage.ACTION => actionPage,
            DialoguePage.CHAT => chatPage,
            DialoguePage.GLYPH => glyphPage,
            DialoguePage.FRIENDSHIP => friendshipPage,
            _ => actionPage,
        };

        StartCoroutine(currentPage.Show());
    }
}
