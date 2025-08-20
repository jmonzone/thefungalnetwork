using System.Collections.Generic;
using UnityEngine;

public class PartyUI : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;

    private List<PartyPageUI> partyPages = new List<PartyPageUI>();

    private void Awake()
    {
        GetComponentsInChildren(true, partyPages);

        foreach (var page in partyPages)
        {
            page.Initialize();
            page.OnPartyReady += Page_OnPartyReady;
        }

        var viewController = GetComponent<ViewController>();
        viewController.OnFadeInStart += ViewController_OnFadeInStart;
    }

    private void OnEnable()
    {
        partyReference.OnPartyPhaseChanged += PartyReference_OnPartyPhaseChanged;
    }

    private void OnDisable()
    {
        partyReference.OnPartyPhaseChanged -= PartyReference_OnPartyPhaseChanged;
    }

    private void Page_OnPartyReady(PartyData party)
    {
        StartCoroutine(partyReference.StartParty(party));
    }

    private void PartyReference_OnPartyPhaseChanged(PartyPhase phase)
    {
        if (phase == PartyPhase.DOORS_OPEN) StartCoroutine(partyReference.DoorsOpenRoutine());
    }

    private void ViewController_OnFadeInStart()
    {
        var i = 0;
        foreach (var page in partyPages)
        {
            if (i < partyReference.Parties.Count)
            {
                page.SetParty(partyReference.Parties[i]);
                page.gameObject.SetActive(true);
            }
            else
            {
                page.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
