using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class ViewReference : ScriptableObject
{
    public event UnityAction OnShow;
    public event UnityAction OnRequestHide;
    public event UnityAction OnHidden;

    public void RequestHide()
    {
        OnRequestHide?.Invoke();
    }

    public void Show()
    {
        OnShow?.Invoke();
    }

    public void OnViewHidden()
    {
        OnHidden?.Invoke();
    }
}