using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PartyPageUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnitListReference unitList;
    [SerializeField] private BuildReference build;
    [SerializeField] private Image partyImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button readyButton;

    [Header("Runtime")]
    [SerializeField] private PartyData party;

    private List<PartyRequirementUI> partyRequirements;
    private List<PartyInviteUI> partyInvites;

    public event UnityAction<PartyData> OnPartyReady;

    private void Awake()
    {
        readyButton.onClick.AddListener(() => OnPartyReady?.Invoke(party));
    }

    public void Initialize()
    {
        partyRequirements = new List<PartyRequirementUI>();
        partyInvites = new List<PartyInviteUI>();

        GetComponentsInChildren(true, partyRequirements);
        GetComponentsInChildren(true, partyInvites);
    }

    public void SetParty(PartyData party)
    {
        this.party = party;

        partyImage.sprite = party.Sprite;
        nameText.text = party.Name;
        levelText.text = $"Party Level {party.Level}";
        descriptionText.text = party.Description;

        var allRequirementsMet = true;

        var i = 0;
        foreach(var requirement in party.Requirements)
        {
            var requirementMet = requirement.Type switch
            {
                PartyRequirementType.BUILD => build.Contains(requirement.Item),
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

        i = 0;
        foreach(var unit in unitList.Units)
        {
            partyInvites[i].SetUnit(unit);
            i++;
        }

        while (i < partyInvites.Count)
        {
            partyInvites[i].Clear();
            i++;
        }
    }
}
