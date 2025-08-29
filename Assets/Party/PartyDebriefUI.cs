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
    [SerializeField] private TextMeshProUGUI scoreText;
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
        scoreText.text = $"{photoReference.AllPhotos.Count} photos taken";
    }
}
