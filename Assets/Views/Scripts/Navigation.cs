using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class Navigation : ScriptableObject
{
    [SerializeField] private List<ViewReference> views;

    //todo: keep track of scene for scene level navigation
    private int buildIndex = 0;

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
        foreach (var view in views)
        {
            view.OnRequestShow += () =>
            {
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

            };

            view.OnHidden += () =>
            {
                currentView.Show();
                OnNavigated?.Invoke();
            };
        }
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
            targetView.RequestShow();
        }

    }
}