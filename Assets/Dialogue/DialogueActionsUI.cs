using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueActionsUI : DialoguePageUI
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private DancefloorReference dancefloorReference;

    [SerializeField] private FadeCanvasGroup actionButtons;
    [SerializeField] private TypewriterEffect dialogueTypewriter;

    [SerializeField] private Button chatButton;
    [SerializeField] private Button photoButton;
    [SerializeField] private Button giveButton;
    [SerializeField] private Button followButton;
    [SerializeField] private Button actionButton;

    protected override void Awake()
    {
        base.Awake();
        chatButton.onClick.AddListener(dialogue.StartChat);
        photoButton.onClick.AddListener(dialogue.StartPhoto);
        giveButton.onClick.AddListener(dialogue.StartGive);
        followButton.onClick.AddListener(dialogue.StartFollow);
        actionButton.onClick.AddListener(UseAction);
    }

    public override void Show()
    {
        base.Show();
        actionButtons.Hide();

        var intro = dialogue.Unit.Instance.RandomDialogue;
        StartCoroutine(dialogueTypewriter.TypeRoutine(intro.Text, () => StartCoroutine(actionButtons.FadeIn())));
    }

    private void UseAction()
    {
        var unit = dialogue.Unit;

        InvokeClose();

        if (unit.Instance.Job == Job.DANCER)
        {
            var units = new List<UnitController> { playerReference.Player, unit };
            dancefloorReference.StartDancefloor(units.Select(unit => unit.GetComponent<UnitDance>()).ToList());
        }
    }
}
