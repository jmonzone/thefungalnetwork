using System.Collections;
using TMPro;
using UnityEngine;

public class Results : MonoBehaviour
{
    [SerializeField] private GameReference game;
    [SerializeField] private Navigation navigation;

    [SerializeField] private ViewReference resultsView;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;

    [SerializeField] private FadeCanvasGroup continueButton;
    [SerializeField] private MoveCharacterJoystick moveCharacterJoystick;

    private void OnEnable()
    {
        game.OnGameComplete += OnGameComplete;
    }

    private void OnDisable()
    {
        game.OnGameComplete -= OnGameComplete;
    }

    private void OnGameComplete()
    {
        StartCoroutine(OnGameCompleteRoutine());
    }

    private IEnumerator OnGameCompleteRoutine()
    {
        moveCharacterJoystick.enabled = false;

        yield return new WaitForSeconds(1.5f);

        headerText.color = game.ClientPlayer.IsWinner ? winColor : loseColor;
        headerText.text = game.ClientPlayer.IsWinner ? "Bog Unclogged" : "Bogged Down";

        navigation.Navigate(resultsView);

        yield return new WaitForSeconds(3f);
        yield return continueButton.FadeIn(0.5f);
    }
}
