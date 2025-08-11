using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UIReference : ScriptableObject
{
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference targetView;

    public event UnityAction OnShow;
    public event UnityAction OnClose;

    public void Show()
    {
        if (navigation.CurrentView != targetView)
        {
            navigation.Navigate(targetView);
            OnShow?.Invoke();
        }
    }

    public void Close()
    {
        if (navigation.CurrentView == targetView)
        {
            navigation.GoBack();
            OnClose?.Invoke();
        }
    }
}

[CreateAssetMenu]
public class SpellbookReference : UIReference
{
    [SerializeField] private List<Item> items;

    public List<Item> Items => items;
}
