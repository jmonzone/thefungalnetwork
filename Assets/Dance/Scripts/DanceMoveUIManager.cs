using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DanceMoveUIManager : MonoBehaviour
{
    private FadeCanvasGroup fadeCanvasGroup;
    private List<DanceMoveUI> moveViews = new List<DanceMoveUI>();

    public void Initialize()
    {
        fadeCanvasGroup = GetComponent<FadeCanvasGroup>();

        moveViews = new List<DanceMoveUI>();
        GetComponentsInChildren(true, moveViews);
    }

    public IEnumerator Show(UnitDance unit, List<DanceMoveInstance> moves, UnityAction onMoveUsed, UnityAction onMoveComplete)
    {
        yield return fadeCanvasGroup.FadeIn();

        var i = 0;
        foreach (var move in moves)
        {
            moveViews[i].SetMove(move, () =>
            {
                onMoveUsed?.Invoke();

                unit.UseDanceMove(move, () =>
                {
                    onMoveComplete?.Invoke();
                });
            });
            moveViews[i].gameObject.SetActive(true);
            i++;
        }

        while (i < moveViews.Count)
        {
            moveViews[i].gameObject.SetActive(false);
            i++;
        }
    }

    public void ToggleInteractable(bool value)
    {
        fadeCanvasGroup.SetInteractable(value);
    }
}

