using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class SceneNavigation : ScriptableObject
{
    [SerializeField] private int buildIndex;
    public int BuildIndex => buildIndex;

    public event UnityAction OnSceneFadeOut;
    public event UnityAction OnSceneFadeIn;
    public event UnityAction OnSceneLoaded;
    public event UnityAction OnSceneNavigationRequest;

    private const float FADE_TRANSITION_DURATION = 0.5f;

    public void Initialize()
    {
        this.buildIndex = SceneManager.GetActiveScene().buildIndex;
        OnSceneFadeIn?.Invoke();
    }

    public void NavigateToScene(int buildIndex)
    {
        this.buildIndex = buildIndex;
        OnSceneNavigationRequest?.Invoke();
    }

    public IEnumerator NavigateToSceneRoutine(FadeCanvasGroup screenFade)
    {
        OnSceneFadeOut?.Invoke();
        yield return screenFade.FadeIn(FADE_TRANSITION_DURATION);

        Debug.Log("loading scene");
        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("scene loaded");
        OnSceneLoaded?.Invoke();

        yield return screenFade.FadeOut(FADE_TRANSITION_DURATION);
        OnSceneFadeIn?.Invoke();
    }

}
