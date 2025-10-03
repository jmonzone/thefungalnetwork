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

    public IEnumerator Show(UnitController unit, List<DanceMoveInstance> moves, UnityAction onMoveUsed, UnityAction onMoveComplete)
    {
        yield return fadeCanvasGroup.FadeIn();

        var i = 0;
        foreach (var move in moves)
        {
            moveViews[i].SetMove(move, () =>
            {
                fadeCanvasGroup.SetInteractable(false);
                onMoveUsed?.Invoke();

                unit.GetComponent<UnitDance>().UseDanceMove(move, () =>
                {
                    fadeCanvasGroup.SetInteractable(true);
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
}
