using System.Collections;
using System.Collections.Generic;
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
        yield return new WaitForSeconds(5f);
        yield return fadeCanvasGroup.FadeOut();
    }


}
