using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class DialoguePageUI : MonoBehaviour
{
    [SerializeField] protected DialogueReference dialogue;
    [SerializeField] protected FadeCanvasGroup fadeCanvasGroup;

    public event UnityAction OnClose;

    protected virtual void Awake()
    {

    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    public virtual IEnumerator Show()
    {
        if (!fadeCanvasGroup.IsVisible) yield return fadeCanvasGroup.FadeIn();
    }

    public virtual void Hide()
    {
        fadeCanvasGroup.gameObject.SetActive(false);
    }

    protected void InvokeClose()
    {
        OnClose?.Invoke();
    }
}
