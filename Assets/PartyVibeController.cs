using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyVibeController : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private Slider slider;

    private void Awake()
    {
        slider.minValue = 0;
        slider.maxValue = 100;
    }

    private void OnEnable()
    {
        partyReference.OnScoreChanged += PartyReference_OnScoreChanged;
    }

    private void OnDisable()
    {
        partyReference.OnScoreChanged -= PartyReference_OnScoreChanged;
    }

    private void PartyReference_OnScoreChanged()
    {
        slider.value = partyReference.Score;
    }

}
