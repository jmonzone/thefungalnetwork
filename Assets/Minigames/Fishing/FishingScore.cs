using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishingScore : MonoBehaviour
{
    [SerializeField] private CastFishingNetButton fishingNetButton;
    [SerializeField] private TextMeshProUGUI scoreText;

    private int score;

    private void Awake()
    {
        fishingNetButton.OnFishCaught += FishingNetButton_OnFishCaught;
    }

    private void FishingNetButton_OnFishCaught(IEnumerable<FishController> fishControllers)
    {
        foreach(var fish in fishControllers)
        {
            score++;
            scoreText.text = score.ToString();
        }
    }
}
