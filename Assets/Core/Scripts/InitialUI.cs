using UnityEngine;
using UnityEngine.Events;

public class InitialUI : MonoBehaviour
{
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference initalUI;

    public event UnityAction OnInitialUIShown;

    private void OnEnable()
    {
        sceneNavigation.OnSceneFadeIn += ShowInitialUI;
    }

    private void OnDisable()
    {
        sceneNavigation.OnSceneFadeIn -= ShowInitialUI;
    }

    private void ShowInitialUI()
    {
        navigation.Navigate(initalUI);
        OnInitialUIShown?.Invoke();
    }
}
