using UnityEngine;

public class InitialUI : MonoBehaviour
{
    [SerializeField] private SceneNavigation sceneNavigation;
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
        initalUI.RequestShow();
    }
}
