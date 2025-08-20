using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PartyPageUI : MonoBehaviour
{
    [SerializeField] private BuildSystem build;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image partyImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button readyButton;
    [SerializeField] private PartyData party;

    private List<PartyRequirementUI> partyRequirements;

    public event UnityAction<PartyData> OnPartyReady;

    private void Awake()
    {
        readyButton.onClick.AddListener(() => OnPartyReady?.Invoke(party));
    }

    public void Initialize()
    {
        partyRequirements = new List<PartyRequirementUI>();
        GetComponentsInChildren(true, partyRequirements);
    }

    public void SetParty(PartyData party)
    {
        this.party = party;

        nameText.text = party.Name;
        levelText.text = $"Party Level {party.Level}";
        partyImage.sprite = party.Sprite;
        descriptionText.text = party.Description;

        var allRequirementsMet = true;

        var i = 0;
        foreach(var requirement in party.Requirements)
        {
            var requirementMet = requirement.Type switch
            {
                PartyRequirementType.MUSIC => build.Contains(requirement.Item),
                PartyRequirementType.LIGHTS => build.Contains(requirement.Item),
                PartyRequirementType.CULTURE => build.CulturePoints >= party.CulturePoints,
                _ => false,
            };

            allRequirementsMet &= requirementMet;

            partyRequirements[i].SetRequirement(requirement, requirementMet);
            partyRequirements[i].gameObject.SetActive(true);
            i++;
        }

        readyButton.interactable = allRequirementsMet;

        while (i < partyRequirements.Count)
        {
            partyRequirements[i].gameObject.SetActive(false);
            i++;
        }
    }
}
