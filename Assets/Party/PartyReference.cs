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
    [SerializeField] private PartyPhase partyPhase;
    [SerializeField] private float currentTimer;
    [SerializeField] private float guestTimer;

    private float numberOfStages = 4;

    public List<PartyData> Parties => parties;

    public PartyData CurrentParty => currentParty;
    public PartyPhase PartyPhase => partyPhase;
    public float CurrentTimer => currentTimer;

    public event UnityAction OnPartyStarted;
    public event UnityAction<PartyPhase> OnPartyPhaseChanged;
    public event UnityAction OnGuestArrived;

    public IEnumerator StartParty(PartyData party)
    {
        currentParty = party;

        navigation.Navigate(partyHUD);
        OnPartyStarted?.Invoke();

        currentTimer = 0f;


        for(var i = 0; i < numberOfStages; i++)
        {
            partyPhase = (PartyPhase)i;
            OnPartyPhaseChanged?.Invoke(partyPhase);

            var stageDuration = party.Duration / numberOfStages;

            while (currentTimer < (i + 1) * stageDuration)
            {
                currentTimer += Time.deltaTime;
                yield return null;
            }
        }
    }

    public IEnumerator DoorsOpenRoutine()
    {
        var stageDuration = currentParty.Duration / numberOfStages;

        // Initial delay before the first guest arrives
        float initialDelay = UnityEngine.Random.Range(0.5f, 2f);
        yield return new WaitForSeconds(initialDelay);

        // Spread guest arrivals across the stage duration
        int guestsToSpawn = currentParty.NumberOfGuests;
        float avgInterval = stageDuration / (guestsToSpawn + 1);

        for (int i = 0; i < guestsToSpawn; i++)
        {
            // Add slight random variation to the interval
            float randomizedInterval = avgInterval * UnityEngine.Random.Range(0.8f, 1.2f);
            yield return new WaitForSeconds(randomizedInterval);

            Debug.Log("Spawn Guest " + (i + 1));
            OnGuestArrived?.Invoke();
        }
    }


}
