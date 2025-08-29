using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// todo: centralize with SceneNavigation
[CreateAssetMenu]
public class Navigation : ScriptableObject
{
    [SerializeField] private List<ViewReference> views;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private List<ViewReference> history;

    private ViewReference currentView;
    public ViewReference CurrentView => currentView;
    public int HistoryCount => history.Count;

    public event UnityAction OnNavigated;

    public void Reset()
    {
        currentView = null;
        history = new List<ViewReference>();

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
        history.Add(view);
    }

    public void GoBack()
    {
        if (history.Count > 1)
        {
            history.RemoveAt(history.Count - 1);
            var targetView = history.Last();

            if (targetView)
            {
                history.RemoveAt(history.Count - 1);
                Navigate(targetView);
            }
        }
    }

    public void GoBack(int steps = 1)
    {
        if (history.Count > steps)
        {
            ViewReference targetView = null;
            history.RemoveAt(history.Count - 1);

            while (steps > 0)
            {
                targetView = history.Last();
                history.RemoveAt(history.Count - 1);
                steps--;
            }

            if (targetView) Navigate(targetView);
        }
    }
}