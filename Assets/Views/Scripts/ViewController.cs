using UnityEngine;

public class ViewController : MonoBehaviour
{
    [SerializeField] private ViewReference viewReference;
    private FadeCanvasGroup canvas;

    private const float FADE_TRANSITION_DURATION = 0.25f;

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
        StartCoroutine(canvas.FadeIn(FADE_TRANSITION_DURATION));
    }

    private void Hide()
    {
        StartCoroutine(canvas.FadeOut(FADE_TRANSITION_DURATION, viewReference.OnViewHidden));
    }
}
