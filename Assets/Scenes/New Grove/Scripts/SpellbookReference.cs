using UnityEngine;
using UnityEngine.Events;

public abstract class UIReference : ScriptableObject
{
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference dialogueView;

    public event UnityAction OnShow;
    public event UnityAction OnClose;

    public void Show()
    {
        if (navigation.CurrentView != dialogueView)
        {
            navigation.Navigate(dialogueView);
            OnShow?.Invoke();
        }
    }

    public void Close()
    {
        if (navigation.CurrentView == dialogueView)
        {
            navigation.GoBack();
            OnClose?.Invoke();
        }
    }
}

[CreateAssetMenu]
public class SpellbookReference : UIReference
{
}
