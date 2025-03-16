using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private Color playerColor;
    [SerializeField] private Color opponentColor;

    private List<Image> images = new List<Image>();

    private void Awake()
    {
        GetComponentsInChildren(includeInactive: true, images);
    }

    private void Start()
    {
        PufferballMinigame_OnScoreUpdated();
    }

    private void OnEnable()
    {
        pufferball.OnScoreUpdated += PufferballMinigame_OnScoreUpdated;
        PufferballMinigame_OnScoreUpdated();
    }

    private void OnDisable()
    {
        pufferball.OnScoreUpdated -= PufferballMinigame_OnScoreUpdated;
    }

    private void PufferballMinigame_OnScoreUpdated()
    {
        for(var i = 0; i < pufferball.CurrentScore; i++)
        {
            images[i].color = playerColor;
            images[i].rectTransform.sizeDelta = Vector2.one * 25f;

        }

        for(var i = images.Count - pufferball.OpponentScore; i < images.Count; i++)
        {
            images[i].color = opponentColor;
            images[i].rectTransform.sizeDelta = Vector2.one * 25f;
        }
    }
}
