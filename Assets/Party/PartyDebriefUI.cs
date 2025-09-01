using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyDebriefUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private PhotoReference photoReference;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference gameplayView;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI rankingText;
    [SerializeField] private Color rank1Color;
    [SerializeField] private Color rank2Color;
    [SerializeField] private Color rank3Color;
    [SerializeField] private Color rank4Color;
    [SerializeField] private Color rank5Color;
    [SerializeField] private Color rank6Color;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI guestsText;
    [SerializeField] private TextMeshProUGUI sporesText;

    [SerializeField] private TextMeshProUGUI photoText;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        continueButton.onClick.AddListener(() => navigation.Navigate(gameplayView));
    }

    private void OnEnable()
    {
        partyReference.OnPartyComplete += PartyReference_OnPartyComplete;
    }

    private void OnDisable()
    {
        partyReference.OnPartyComplete -= PartyReference_OnPartyComplete;
    }

    private void PartyReference_OnPartyComplete()
    {
        switch (partyReference.Score)
        {
            case > 100:
                rankingText.text = "S";
                rankingText.color = rank1Color;
                break;
            case > 75:
                rankingText.text = "A";
                rankingText.color = rank2Color;
                break;
            case > 50:
                rankingText.text = "B";
                rankingText.color = rank3Color;
                break;
            case > 25:
                rankingText.text = "C";
                rankingText.color = rank4Color;
                break;
            case > 10:
                rankingText.text = "D";
                rankingText.color = rank5Color;
                break;
            default:
                rankingText.text = "F";
                rankingText.color = rank6Color;
                break;
        }

        scoreText.text = $"{partyReference.Score} party points!";
        guestsText.text = $"{partyReference.CurrentParty.Guests.Count} of your friends came";
        sporesText.text = $"{partyReference.SporesCollected} spores collected";
        photoText.text = $"{photoReference.AllPhotos.Count} photos taken";
    }
}
