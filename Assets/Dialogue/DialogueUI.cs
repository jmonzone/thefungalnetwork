using UnityEngine;
using UnityEngine.UI;

public enum DialoguePage
{
    ACTION,
    CHAT,
    FRIENDSHIP
}


public class DialogueUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private UnitListReference unitListReference;

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

        actionPage.Hide();
        chatPage.Hide();
        friendshipPage.Hide();

        chatPage.OnClose += ChatPage_OnClose;
        friendshipPage.OnClose += FriendshipPage_OnClose;

        closeButton.onClick.AddListener(() => dialogue.CompleteDialogue());
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

    private void ChatPage_OnClose()
    {
        if (dialogue.Dialogue.Type == DialogueType.CHAT)
        {
            ShowPage(DialoguePage.FRIENDSHIP);
        }
        else if (dialogue.Dialogue.Type == DialogueType.FRIEND)
        {
            var selectUnit = dialogue.Unit;
            dialogue.CompleteDialogue();
            unitListReference.SelectFungal(selectUnit.Instance);
        }
        else
        {
            dialogue.CompleteDialogue();
        }
    }

    private void FriendshipPage_OnClose()
    {
        ShowPage(DialoguePage.CHAT);
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
