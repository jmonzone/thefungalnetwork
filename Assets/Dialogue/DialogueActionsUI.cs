using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueActionsUI : DialoguePageUI
{
    [SerializeField] private PlayerReference playerReference;

    [SerializeField] private FadeCanvasGroup actionButtons;
    [SerializeField] private TypewriterEffect dialogueTypewriter;

    [SerializeField] private DialogueChatUI chatUI;

    [SerializeField] private Button chatButton;
    [SerializeField] private Button photoButton;
    [SerializeField] private Button giveButton;
    [SerializeField] private Button followButton;

    [SerializeField] private Button actionButton;
    [SerializeField] private Image actionBackground;
    [SerializeField] private Image actionImage;
    [SerializeField] private TextMeshProUGUI actionText;

    private UnitController unit;

    protected override void Awake()
    {
        base.Awake();
        chatButton.onClick.AddListener(() =>
        {
            dialogue.StartDialogue(unit, unit.Instance.RandomDialogue);
        });

        photoButton.onClick.AddListener(dialogue.StartPhoto);
        giveButton.onClick.AddListener(dialogue.StartGive);
        followButton.onClick.AddListener(dialogue.StartFollow);
        actionButton.onClick.AddListener(UseAction);
    }

    public override IEnumerator Show()
    {
        yield return base.Show();
        actionButtons.Hide();

        unit = dialogue.Unit;

        if (unit.Instance.Job)
        {
            actionBackground.color = unit.Instance.Job.ActionColor;
            actionImage.sprite = unit.Instance.Job.ActionSprite;
            actionText.text = unit.Instance.Job.ActionName;
        }

        actionButton.gameObject.SetActive(unit.Instance.Job);

        var intro = unit.Instance.RandomDialogue;
        yield return dialogueTypewriter.TypeRoutine(intro.Text, () => StartCoroutine(actionButtons.FadeIn()));
    }

    private void UseAction()
    {
        // Collect all possible units (player, dialogue unit, followers)
        var allUnits = new List<UnitController>
        {
            playerReference.Player,
            dialogue.Unit
        };

        InvokeClose();

        allUnits.AddRange(playerReference.Player
            .GetComponent<UnitFollow>()
            .Followers
            .Select(f => f.Controller));

        // Deduplicate, but keep player first
        var uniqueUnits = allUnits
            .Distinct()
            .OrderBy(u => u != playerReference.Player) // ensures player stays first
            .ToList();

        // Stop all followers
        foreach (var unit in uniqueUnits)
        {
            unit.GetComponent<UnitFollow>().StopFollowing();
        }

        //unit.Instance.Job.Activity.StartActivity(uniqueUnits);
    }

}
