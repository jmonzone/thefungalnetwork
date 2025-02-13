using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishingScore : MonoBehaviour
{
    [SerializeField] private FishManager fishManager;
    [SerializeField] private TextMeshProUGUI scoreText;

    private int score;

    private void Awake()
    {
        fishManager.OnFishCaught += FishingNetButton_OnFishCaught;
    }

    private void FishingNetButton_OnFishCaught()
    {
        score++;
        scoreText.text = score.ToString();
    }
}
