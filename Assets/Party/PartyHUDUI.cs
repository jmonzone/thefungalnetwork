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

    private float numberOfStages = 4;

    public event UnityAction OnGuestArrived;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            navigation.GoBack(2);
        });
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
        slider.minValue = 0;
        slider.maxValue = partyReference.CurrentParty.Duration;

        StartCoroutine(PartyRoutine());
    }

    private void Update()
    {
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

    public IEnumerator PartyRoutine()
    {
        currentTimer = 0f;
        for (var i = 0; i < numberOfStages; i++)
        {
            partyPhase = (PartyPhase)i;
            if (partyPhase == PartyPhase.DOORS_OPEN)
            {
                StartCoroutine(DoorsOpenRoutine());
            }

            var stageDuration = partyReference.CurrentParty.Duration / numberOfStages;

            while (currentTimer < (i + 1) * stageDuration)
            {
                currentTimer += Time.deltaTime;
                yield return null;
            }
        }
    }

    public IEnumerator DoorsOpenRoutine()
    {
        var stageDuration = partyReference.CurrentParty.Duration / numberOfStages;

        // Initial delay before the first guest arrives
        float initialDelay = Random.Range(0.5f, 2f);
        yield return new WaitForSeconds(initialDelay);

        // Spread guest arrivals across the stage duration
        int guestsToSpawn = partyReference.CurrentParty.NumberOfGuests;
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
