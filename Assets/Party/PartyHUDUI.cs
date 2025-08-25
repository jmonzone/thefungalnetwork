using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PartyHUDUI : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Navigation navigation;


    [SerializeField] private PartyPhase partyPhase;
    [SerializeField] private float currentTimer;
    [SerializeField] private float guestTimer;
    [SerializeField] private bool partyStarted;

    private float numberOfStages = 3;

    public event UnityAction<Unit> OnGuestArrived;

    private void Awake()
    {
        partyStarted = false;
        closeButton.onClick.AddListener(() =>
        {
            navigation.GoBack(2);
        });
    }

    private void Update()
    {
        if (partyStarted)
        {
            currentTimer += Time.deltaTime;
            slider.value = currentTimer;
            phaseText.text = partyPhase switch
            {
                PartyPhase.DOORS_OPEN => "Doors Open",
                PartyPhase.COCKTAIL_HOUR => "Meet the Fungals!",
                PartyPhase.EVENT => "Main Event",
                PartyPhase.WIND_DOWN => "Slow things down",
                PartyPhase.CLEANUP => "Clean up time!",
                _ => "error",
            };
        }

    }

    private void OnEnable()
    {
        partyReference.OnPartyStarted += PartyReference_OnPartyStarted;
    }

    private void OnDisable()
    {
        partyReference.OnPartyStarted -= PartyReference_OnPartyStarted;
    }

    private void PartyReference_OnPartyStarted()
    {
        partyStarted = true;
        currentTimer = 0;
        slider.minValue = 0;
        slider.maxValue = GetTotalPartyDuration();

        StartCoroutine(PartyRoutine());
    }

    private float GetPhaseDuration(PartyPhase phase)
    {
        return phase switch
        {
            PartyPhase.DOORS_OPEN => 7.5f,
            _ => 15f,
        };
    }

    private float GetTotalPartyDuration()
    {
        float total = 0f;
        for (int i = 0; i < numberOfStages; i++)
        {
            total += GetPhaseDuration((PartyPhase)i);
        }
        return total;
    }



    // Pass in explicit phase durations
    public IEnumerator PartyRoutine()
    {
        for (int i = 0; i < numberOfStages; i++)
        {
            partyPhase = (PartyPhase)i;
            float phaseDuration = GetPhaseDuration(partyPhase);

            // Run phase-specific logic
            switch (partyPhase)
            {
                case PartyPhase.DOORS_OPEN:
                    yield return DoorsOpenRoutine(phaseDuration);
                    break;

                default:
                    yield return new WaitForSeconds(phaseDuration);
                    break;
            }
        }

        partyStarted = false;
        partyReference.StopParty();
    }

    private IEnumerator DoorsOpenRoutine(float duration)
    {
        // Initial delay before the first guest arrives
        float initialDelay = Random.Range(0.5f, 2f);
        yield return new WaitForSeconds(initialDelay);

        int guestsToSpawn = partyReference.CurrentParty.Guests.Count;
        if (guestsToSpawn == 0)
            yield break;

        // Spread arrivals across the given phase duration
        float avgInterval = duration / (guestsToSpawn + 1);

        for (int i = 0; i < guestsToSpawn; i++)
        {
            float randomizedInterval = avgInterval * Random.Range(0.8f, 1.2f);
            yield return new WaitForSeconds(randomizedInterval);

            Debug.Log("Spawn Guest " + (i + 1));
            OnGuestArrived?.Invoke(partyReference.CurrentParty.Guests[i]);
        }
    }
}
