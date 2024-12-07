using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class SceneNavigation : ScriptableObject
{
    private int buildIndex;

    public event UnityAction OnSceneLoaded;
    public event UnityAction OnSceneNavigationRequest;

    public void LoadScene()
    {
        OnSceneLoaded?.Invoke();
    }

    public void NavigateToScene(int buildIndex)
    {
        this.buildIndex = buildIndex;
        OnSceneNavigationRequest?.Invoke();
    }

    public IEnumerator NavigateToSceneRoutine(FadeCanvasGroup screenFade)
    {
        yield return screenFade.FadeIn(2f);

        Debug.Log("loading scene");
        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("scene loaded");

        yield return screenFade.FadeOut(2f);
    }

}
