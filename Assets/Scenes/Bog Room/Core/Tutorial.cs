using System.Collections;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FadeCanvasGroup fadeCanvasGroup;
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable()
    {
        arena.OnIntroComplete += Arena_OnIntroComplete;
        arena.OnMinionAssigned += Arena_OnMinionAssigned;
    }

    private void Arena_OnMinionAssigned()
    {
        SetInformation("You have been possessed");
    }

    private void Arena_OnIntroComplete()
    {
        SetInformation("Collect the mushrooms in order to escape");
    }

    private void SetInformation(string text)
    {
        this.text.text = text;

        StartCoroutine(Arena_OnIntroCompleteCoroutine());

    }

    private IEnumerator Arena_OnIntroCompleteCoroutine()
    {
        yield return fadeCanvasGroup.FadeIn();
        yield return new WaitForSeconds(2f);
        yield return fadeCanvasGroup.FadeOut();
    }
}
