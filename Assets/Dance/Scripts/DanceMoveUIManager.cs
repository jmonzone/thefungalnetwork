using System.Collections.Generic;
using UnityEngine;

public class DanceMoveUIManager : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private FadeCanvasGroup fadeCanvasGroup;
    private List<DanceMoveUI> moveViews = new List<DanceMoveUI>();

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        fadeCanvasGroup = GetComponent<FadeCanvasGroup>();

        moveViews = new List<DanceMoveUI>();
        GetComponentsInChildren(true, moveViews);

        foreach (var view in moveViews)
        {
            view.OnDanceMoveStart += () => canvasGroup.interactable = false;
            view.OnDanceMoveComplete += () => canvasGroup.interactable = true;
        }
    }

    public void Show(UnitDance dancer)
    {
        gameObject.SetActive(true);
        StartCoroutine(fadeCanvasGroup.FadeIn());

        var i = 0;
        foreach (var move in dancer.DanceMoves)
        {
            moveViews[i].SetMove(dancer, move);
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
