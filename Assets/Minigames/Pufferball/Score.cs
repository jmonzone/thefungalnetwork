using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private PufferballMinigame pufferballMinigame;
    [SerializeField] private Color playerColor;
    [SerializeField] private Color opponentColor;

    private List<Image> images = new List<Image>();

    private void Awake()
    {
        GetComponentsInChildren(includeInactive: true, images);
        pufferballMinigame.OnScoreUpdated += PufferballMinigame_OnScoreUpdated;
    }

    private void PufferballMinigame_OnScoreUpdated()
    {
        for(var i = 0; i < pufferballMinigame.CurrentScore; i++)
        {
            images[i].color = playerColor;
        }

        for(var i = images.Count - pufferballMinigame.OpponentScore; i < images.Count; i++)
        {
            images[i].color = opponentColor;
        }
    }
}
