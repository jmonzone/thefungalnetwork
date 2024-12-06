using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
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

    public void Initialize()
    {
        currentView = null;
        history = new Stack<ViewReference>();

        foreach (var view in views)
        {
            view.OnOpened += () =>
            {
                if (currentView) currentView.Close();
                SetCurrentView(view);
            };
        }
    }


    private void SetCurrentView(ViewReference view)
    {
        currentView = view;
        history.Push(view);

        //todo: if this fails, undo logic
        OnNavigated?.Invoke();
    }

    public void GoBack()
    {
        if (history.Count > 1)
        {
            history.Pop();
            var targetView = history.Pop();
            targetView.Open();
        }
        else
        {
            SceneManager.LoadScene(buildIndex);
        }

    }
}