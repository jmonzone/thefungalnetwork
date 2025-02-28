using TMPro;
using UnityEngine;

public class Results : MonoBehaviour
{
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private Navigation navigation;

    [SerializeField] private ViewReference resultsView;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;

    private void OnEnable()
    {
        pufferball.OnScoreUpdated += PufferballMinigame_OnScoreUpdated;
    }

    private void OnDisable()
    {
        pufferball.OnScoreUpdated -= PufferballMinigame_OnScoreUpdated;
    }


    private void PufferballMinigame_OnScoreUpdated()
    {
        var isWinner = pufferball.CurrentScore >= 3;
        var isLoser = pufferball.OpponentScore >= 3;

        if (isWinner || isLoser)
        {
            headerText.color = isWinner ? winColor : loseColor;
            headerText.text = isWinner ? "Bog Unclogged" : "Bogged Down";

            navigation.Navigate(resultsView);
        }
    }
}
