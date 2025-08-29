using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PartyPhase
{
    DOORS_OPEN = 0,
    COCKTAIL_HOUR = 1,
    EVENT = 2,
    WIND_DOWN = 3,
    CLEANUP = 4
}

[CreateAssetMenu]
public class PartyReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference partyHUD;
    [SerializeField] private ViewReference debriefView;
    [SerializeField] private UnitListReference unitList;

    [Header("Data")]
    [SerializeField] private List<PartyData> parties;

    [Header("Runtime")]
    [SerializeField] private PartyData currentParty;
    [SerializeField] private bool isActive;
    [SerializeField] private List<UnitController> guests;

    public List<UnitController> Guests => guests;
    public List<PartyData> Parties => parties;

    public PartyData CurrentParty => currentParty;
    public bool IsActive => isActive;

    public event UnityAction OnPartyStarted;
    public event UnityAction OnPartyPaused;
    public event UnityAction OnPartyComplete;

    public event UnityAction OnPartyResumed;

    public void Initialize()
    {
        isActive = false;
        guests = new List<UnitController>();
    }

    public void StartParty(PartyData party)
    {
        isActive = true;
        currentParty = party;

        navigation.Navigate(partyHUD);
        OnPartyStarted?.Invoke();
    }

    public void PauseParty()
    {
        isActive = false;
        OnPartyPaused?.Invoke();
    }

    public void ResumeParty()
    {
        isActive = true;
        OnPartyResumed?.Invoke();
    }

    public void StopParty()
    {
        navigation.Navigate(debriefView);
        //navigation.GoBack(navigation.HistoryCount - 1);
        isActive = false;
        OnPartyComplete?.Invoke();
    }

    public void AddGuest(UnitController guest)
    {
        guests.Add(guest);
    }

    public void ClearGuests()
    {
        foreach (var guest in guests)
        {
            Destroy(guest.gameObject, 1f); // delay for animation
        }

        guests = new List<UnitController>();
    }
}
