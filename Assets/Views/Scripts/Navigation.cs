using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// todo: centralize with SceneNavigation
[CreateAssetMenu]
public class Navigation : ScriptableObject
{
    [SerializeField] private List<ViewReference> views;
    [SerializeField] private SceneNavigation sceneNavigation;

    private ViewReference currentView;
    private Stack<ViewReference> history;

    public Stack<ViewReference> History => history;

    public event UnityAction OnNavigated;

    public void Reset()
    {
        currentView = null;
        history = new Stack<ViewReference>();
    }

    public void Initialize()
    {
        Reset();
        sceneNavigation.OnSceneNavigationRequest += () =>
        {
            if (currentView) currentView.RequestHide();
        };

        foreach (var view in views)
        {
            view.OnHidden += () =>
            {
                if (currentView) currentView.Show();
                OnNavigated?.Invoke();
            };
        }
    }

    public void Navigate(ViewReference view)
    {
        var previousView = currentView;
        SetCurrentView(view);

        if (previousView)
        {
            previousView.RequestHide();
        }

        currentView.Show();
    }


    private void SetCurrentView(ViewReference view)
    {
        currentView = view;
        history.Push(view);
    }

    public void GoBack()
    {
        if (history.Count > 1)
        {
            history.Pop();
            var targetView = history.Pop();
            Navigate(targetView);
        }

    }
}