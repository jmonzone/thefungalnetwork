using UnityEngine;

public class InitialUI : MonoBehaviour
{
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference initalUI;

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
        Debug.Log("showing");
        navigation.Navigate(initalUI);
    }
}
