using UnityEngine;

[RequireComponent(typeof(FadeCanvasGroup))]
public class ViewController : MonoBehaviour
{
    [SerializeField] private ViewReference viewReference;
    private FadeCanvasGroup canvas;

    private void Awake()
    {
        canvas = GetComponent<FadeCanvasGroup>();
    }
    private void OnEnable()
    {
        viewReference.OnOpened += Show;
        viewReference.OnClosed += Hide;
    }

    private void OnDisable()
    {
        viewReference.OnOpened -= Show;
        viewReference.OnClosed -= Hide;
    }

    private void Show()
    {
        StartCoroutine(canvas.FadeIn());
    }

    private void Hide()
    {
        StartCoroutine(canvas.FadeOut());
    }
}
