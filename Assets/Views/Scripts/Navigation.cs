using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Navigation : ScriptableObject
{
    [SerializeField] private List<ViewReference> views;

    private Stack<ViewReference> history;

    private ViewReference currentView;

    public void Initialize()
    {
        history = new Stack<ViewReference>();

        if (views.Count > 0)
        {
            SetCurrentView(views[0]);

            foreach (var view in views)
            {
                view.OnOpened += () =>
                {
                    Debug.Log("Opening");
                    currentView.Close();
                    SetCurrentView(view);
                };
            }
        }
    }

    private void SetCurrentView(ViewReference view)
    {
        currentView = view;
        history.Push(view);
    }

    public void GoBack()
    {
        Debug.Log("Closing");
        history.Pop();
        var targetView = history.Pop();
        targetView.Open();
    }
}