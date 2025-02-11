using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingResults : MonoBehaviour
{
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference resultsView;
    [SerializeField] private FishingTimer fishingTimer;

    private void Awake()
    {
        fishingTimer.OnTimerComplete += FishingTimer_OnTimerComplete;
    }

    private void FishingTimer_OnTimerComplete()
    {
        navigation.Navigate(resultsView);
    }
}
