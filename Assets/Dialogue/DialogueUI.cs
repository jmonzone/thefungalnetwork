using System.Collections.Generic;
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
    [SerializeField] private DancefloorReference dancefloor;

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

        chatPage.OnClose += OnChatPageClosed;
        glyphPage.OnClose += OnChatPageClosed;
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
        if (dialogue.Unit is TreeController)
        {
            ShowPage(DialoguePage.GLYPH);
        }
        else
        {
            ShowPage(DialoguePage.CHAT);
        }
    }

    private void StartDialogue()
    {
        ShowPage(DialoguePage.CHAT);
    }

    private void OnChatPageClosed()
    {
        Debug.Log("do action");

        if (dialogue.Unit.Instance.Job == Job.DANCER)
        {
            var units = new List<UnitController> { dialogue.Unit, playerReference.Player };
            dancefloor.StartDancefloor(units);
        }
        else
        {
            dialogue.CompleteDialogue();
        }
        //if (dialogue.Dialogue.Type == DialogueType.CHAT && dialogue.Unit is FungalController)
        //{
        //    ShowPage(DialoguePage.FRIENDSHIP);
        //}
        //else if (dialogue.Dialogue.Type == DialogueType.FRIEND && friendshipPage.HasLeveledUp && dialogue.Unit.Instance.FriendshipLevel == 2)
        //{
        //    var selectUnit = dialogue.Unit;
        //    dialogue.CompleteDialogue();
        //    unitListReference.SelectFungal(selectUnit.Instance);
        //}
        //else
        //{
        //    dialogue.CompleteDialogue();
        //}
    }

    private void FriendshipPage_OnClose()
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

        currentPage.Show();
    }
}
