using System.Collections;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    [SerializeField] private ViewReference viewReference;
    private FadeCanvasGroup canvas;

    private float transitionDuration = 0.25f;

    private void Awake()
    {
        canvas = GetComponentInChildren<FadeCanvasGroup>(includeInactive: true);
    }

    private void OnEnable()
    {
        viewReference.OnShow += Show;
        viewReference.OnRequestHide += Hide;
    }

    private void OnDisable()
    {
        viewReference.OnShow -= Show;
        viewReference.OnRequestHide -= Hide;
    }

    private void Show()
    {
        StartCoroutine(canvas.FadeIn(transitionDuration));
    }

    private void Hide()
    {
        StartCoroutine(canvas.FadeOut(transitionDuration, viewReference.OnViewHidden));
    }
}
