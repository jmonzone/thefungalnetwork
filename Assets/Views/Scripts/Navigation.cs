using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class Navigation : ScriptableObject
{
    [SerializeField] private List<ViewReference> views;

    private Stack<ViewReference> history;
    public Stack<ViewReference> History => history;

    public event UnityAction OnNavigated;

    private ViewReference currentView;

    public void Initialize(List<ViewReference> initialViews = null)
    {
        history = new Stack<ViewReference>(initialViews ?? new List<ViewReference>());

        views = Utility.LoadAssets<ViewReference>();

        if (views.Count > 0)
        {
            foreach (var view in views)
            {
                view.OnOpened += () =>
                {
                    currentView.Close();
                    SetCurrentView(view);
                };
            }
        }
    }

    //private void PopulateViews()
    //{
    //    views = new List<ViewReference>();
    //    string[] guids = AssetDatabase.FindAssets($"t:{nameof(ViewReference)}");
    //    foreach (string guid in guids)
    //    {
    //        string path = AssetDatabase.GUIDToAssetPath(guid);
    //        var view = AssetDatabase.LoadAssetAtPath<ViewReference>(path);
    //        if (view != null)
    //        {
    //            views.Add(view);
    //        }
    //    }

    //    Debug.Log($"Populated {views.Count} ViewReference assets into {name}");
    //}

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