using System.Collections;
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

    [SerializeField] private FadeCanvasGroup continueButton;
    [SerializeField] private MoveCharacterJoystick moveCharacterJoystick;

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
        headerText.color = pufferball.ClientPlayer.IsWinner ? winColor : loseColor;
        headerText.text = pufferball.ClientPlayer.IsWinner ? "Bog Unclogged" : "Bogged Down";

        navigation.Navigate(resultsView);

        moveCharacterJoystick.enabled = false;

        StartCoroutine(FadeInContinueButton());
    }

    private IEnumerator FadeInContinueButton()
    {
        yield return new WaitForSeconds(3f);
        yield return continueButton.FadeIn(0.5f);
    }
}
