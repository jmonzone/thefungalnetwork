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
        pufferball.OnGameComplete += Pufferball_OnGameComplete;
    }

    private void OnDisable()
    {
        pufferball.OnGameComplete -= Pufferball_OnGameComplete;
    }

    private void Pufferball_OnGameComplete()
    {
        if (pufferball.IsWinner)
        {
            headerText.color = pufferball.IsWinner ? winColor : loseColor;
            headerText.text = pufferball.IsWinner ? "Bog Unclogged" : "Bogged Down";

            navigation.Navigate(resultsView);
        }
    }
}
