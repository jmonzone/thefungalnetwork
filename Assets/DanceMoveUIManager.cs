using System.Collections.Generic;
using UnityEngine;

public class DanceMoveUIManager : MonoBehaviour
{
    private FadeCanvasGroup fadeCanvasGroup;
    private List<DanceMoveUI> moveViews = new List<DanceMoveUI>();

    private void Awake()
    {
        fadeCanvasGroup = GetComponent<FadeCanvasGroup>();
    }

    public void Show(UnitDance dancer)
    {
        moveViews = new List<DanceMoveUI>();
        GetComponentsInChildren(true, moveViews);

        gameObject.SetActive(true);
        StartCoroutine(fadeCanvasGroup.FadeIn());

        var moves = new List<string> { "Die01_SwordAndShield", "Attack04_SwordAndShiled" };

        var i = 0;
        foreach(var move in moveViews)
        {
            move.SetMove(dancer, moves[i]);
            i++;
        }
    }
}
