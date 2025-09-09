using UnityEngine;
using UnityEngine.UI;

public class DialogueActionsUI : DialoguePageUI
{
    [SerializeField] private DialogueReference dialogueReference;

    [SerializeField] private FadeCanvasGroup actionButtons;
    [SerializeField] private TypewriterEffect dialogueTypewriter;

    [SerializeField] private Button chatButton;
    [SerializeField] private Button photoButton;
    [SerializeField] private Button giveButton;
    [SerializeField] private Button followButton;

    private void Awake()
    {
        chatButton.onClick.AddListener(dialogueReference.StartChat);
        photoButton.onClick.AddListener(dialogueReference.StartPhoto);
        giveButton.onClick.AddListener(dialogueReference.StartGive);
        followButton.onClick.AddListener(dialogueReference.StartFollow);
    }

    public override void Show()
    {
        base.Show();
        actionButtons.gameObject.SetActive(false);

        var allIntros = dialogueReference.Unit.Data.Intros;
        var intro = allIntros[Random.Range(0, allIntros.Count)];
        StartCoroutine(dialogueTypewriter.TypeRoutine(intro, () => StartCoroutine(actionButtons.FadeIn())));
    }

    public override void Hide()
    {
        base.Hide();
    }

}
