using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ViewController : MonoBehaviour
{
    [SerializeField] private ViewReference viewReference;
    private FadeCanvasGroup canvas;

    private const float FADE_TRANSITION_DURATION = 0.5f;

    public event UnityAction OnFadeInStart;
    public event UnityAction OnFadeInComplete;
    public event UnityAction OnFadeOutComplete;

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
        StartCoroutine(FadeInView());
    }

    private IEnumerator FadeInView()
    {
        OnFadeInStart?.Invoke();
        yield return canvas.FadeIn(FADE_TRANSITION_DURATION);
        OnFadeInComplete?.Invoke();
    }

    private void Hide()
    {
        StartCoroutine(canvas.FadeOut(FADE_TRANSITION_DURATION, () =>
        {
            viewReference.OnViewHidden();
            OnFadeOutComplete?.Invoke();
        }));
    }
}
