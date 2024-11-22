using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class ViewReference : ScriptableObject
{
    public event UnityAction OnOpened;
    public event UnityAction OnClosed;

    public void Open()
    {
        OnOpened?.Invoke();
    }

    public void Close()
    {
        OnClosed?.Invoke();
    }
}
