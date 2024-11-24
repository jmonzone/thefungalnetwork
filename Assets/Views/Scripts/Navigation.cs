using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class Navigation : ScriptableObject
{
    [SerializeField] private ViewReference currentView;
    [SerializeField] private List<ViewReference> views;

    private Stack<ViewReference> history;
    public Stack<ViewReference> History => history;

    public event UnityAction OnNavigated;


    public void Initialize(ViewReference initialView)
    {
        history = new Stack<ViewReference>();
        SetCurrentView(initialView);

        foreach (var view in views)
        {
            view.OnOpened += () =>
            {
                currentView.Close();
                SetCurrentView(view);
            };
        }
    }


    private void SetCurrentView(ViewReference view)
    {
        currentView = view;
        history.Push(view);
        OnNavigated?.Invoke();
    }

    public void GoBack()
    {
        history.Pop();
        var targetView = history.Pop();
        targetView.Open();
    }
}