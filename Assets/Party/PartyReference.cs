using System;
using System.Collections;
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

    [Header("Data")]
    [SerializeField] private List<PartyData> parties;

    [Header("Runtime")]
    [SerializeField] private PartyData currentParty;

    public List<PartyData> Parties => parties;

    public PartyData CurrentParty => currentParty;

    public event UnityAction OnPartyStarted;

    public void StartParty(PartyData party)
    {
        currentParty = party;

        navigation.Navigate(partyHUD);
        OnPartyStarted?.Invoke();
    }

    public void StopParty()
    {

    }
}
