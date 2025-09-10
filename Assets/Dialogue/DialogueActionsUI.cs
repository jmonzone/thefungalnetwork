using UnityEngine;
using UnityEngine.UI;

public class DialogueActionsUI : DialoguePageUI
{

    [SerializeField] private FadeCanvasGroup actionButtons;
    [SerializeField] private TypewriterEffect dialogueTypewriter;

    [SerializeField] private Button chatButton;
    [SerializeField] private Button photoButton;
    [SerializeField] private Button giveButton;
    [SerializeField] private Button followButton;

    protected override void Awake()
    {
        base.Awake();
        chatButton.onClick.AddListener(dialogue.StartChat);
        photoButton.onClick.AddListener(dialogue.StartPhoto);
        giveButton.onClick.AddListener(dialogue.StartGive);
        followButton.onClick.AddListener(dialogue.StartFollow);
    }

    public override void Show()
    {
        base.Show();
        actionButtons.gameObject.SetActive(false);

        var allIntros = dialogue.Unit.Instance.Data.Intros;
        var intro = allIntros[Random.Range(0, allIntros.Count)];
        StartCoroutine(dialogueTypewriter.TypeRoutine(intro, () => StartCoroutine(actionButtons.FadeIn())));
    }

    public override void Hide()
    {
        base.Hide();
    }

}
