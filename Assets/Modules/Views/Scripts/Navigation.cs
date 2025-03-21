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

        foreach (var view in views)
        {
            view.OnHidden -= View_OnHidden;
        }

        views = new List<ViewReference>();
    }

    public void Initialize()
    {
        views = new List<ViewReference>();
        Reset();
        sceneNavigation.OnSceneNavigationRequest += () =>
        {
            if (currentView) currentView.RequestHide();
        };
    }

    public void InitalizeHistory(IEnumerable<ViewReference> initalViews)
    {
        history = new Stack<ViewReference>(initalViews);
    }

    public void RegisterView(ViewReference view)
    {
        view.OnHidden += View_OnHidden;
        views.Add(view);
    }

    private void View_OnHidden()
    {
        if (currentView) currentView.Show();
        OnNavigated?.Invoke();
    }


    public void Navigate(ViewReference view)
    {
        if (!views.Contains(view))
        {
            RegisterView(view);
        }

        var previousView = currentView;
        SetCurrentView(view);

        if (previousView)
        {
            previousView.RequestHide();
        }
        else
        {
            currentView.Show();
        }
    }


    private void SetCurrentView(ViewReference view)
    {
        currentView = view;
        history.Push(view);
    }

    public void GoBack()
    {
        Debug.Log("going back");
        if (history.Count > 1)
        {
            history.Pop();
            var targetView = history.Pop();
            Navigate(targetView);
        }

    }
}