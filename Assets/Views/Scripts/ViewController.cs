using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ViewController : MonoBehaviour
{
    [SerializeField] private ViewReference viewReference;
    private FadeCanvasGroup canvas;

    private const float FADE_TRANSITION_DURATION = 0.25f;
    public event UnityAction OnViewShowComplete;

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
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        yield return canvas.FadeIn(FADE_TRANSITION_DURATION);
        OnViewShowComplete?.Invoke();
    }

    private void Hide()
    {
        StartCoroutine(canvas.FadeOut(FADE_TRANSITION_DURATION, viewReference.OnViewHidden));
    }
}
