using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Navigation navigation;


    [SerializeField] private PartyPhase partyPhase;

    [SerializeField] private float currentTimer;
    [SerializeField] private float guestTimer;

    [SerializeField] private float doorsOpenDuration = 7.5f;
    [SerializeField] private float defaultPhaseDuration = 15f;

    private float numberOfStages = 4;

    public event UnityAction<PartyPhase> OnPhaseChanged;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            navigation.GoBack(2);
        });
    }

    private void Start()
    {
        partyReference.StartParty(partyReference.Parties[0]);
    }

    private void Update()
    {
        if (partyReference.IsActive)
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
        currentTimer = 0;
        slider.minValue = 0;
        slider.maxValue = GetTotalPartyDuration();

        StartCoroutine(PartyRoutine());
    }

    private float GetPhaseDuration(PartyPhase phase)
    {
        return phase switch
        {
            PartyPhase.DOORS_OPEN => doorsOpenDuration,
            _ => defaultPhaseDuration,
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
            yield return new WaitUntil(() => partyReference.IsActive);

            partyPhase = (PartyPhase)i;
            OnPhaseChanged?.Invoke(partyPhase);
            float phaseDuration = GetPhaseDuration(partyPhase);

            yield return new WaitForSeconds(phaseDuration);
        }

        //partyReference.StopParty();
    }
}
