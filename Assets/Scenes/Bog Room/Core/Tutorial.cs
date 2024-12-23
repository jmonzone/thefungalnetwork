using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FadeCanvasGroup fadeCanvasGroup;

    private void OnEnable()
    {
        arena.OnIntroComplete += Arena_OnIntroComplete;
    }

    private void Arena_OnIntroComplete()
    {
        StartCoroutine(Arena_OnIntroCompleteCoroutine());
    }

    private IEnumerator Arena_OnIntroCompleteCoroutine()
    {
        yield return fadeCanvasGroup.FadeIn();
        yield return new WaitForSeconds(2f);
        yield return fadeCanvasGroup.FadeOut();
    }
}
