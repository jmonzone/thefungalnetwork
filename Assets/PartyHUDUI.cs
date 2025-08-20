using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyHUDUI : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI phaseText;

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
    }

    private void Update()
    {
        slider.value = partyReference.CurrentTimer;
        phaseText.text = partyReference.PartyPhase switch
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
