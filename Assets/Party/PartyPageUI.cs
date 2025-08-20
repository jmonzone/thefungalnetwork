using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyPageUI : MonoBehaviour
{
    [SerializeField] private BuildSystem build;
    [SerializeField] private Item djTable;
    [SerializeField] private Item partyLights;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image partyImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button readyButton;

    private List<PartyRequirementUI> partyRequirements;

    private void Awake()
    {
        partyRequirements = new List<PartyRequirementUI>();
        GetComponentsInChildren(true, partyRequirements);
    }

    public void SetParty(PartyData party)
    {
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
                PartyRequirementType.MUSIC => build.Contains(djTable),
                PartyRequirementType.LIGHTS => build.Contains(partyLights),
                PartyRequirementType.CULTURE => build.CulturePoints >= requirement.CulturePoints,
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
