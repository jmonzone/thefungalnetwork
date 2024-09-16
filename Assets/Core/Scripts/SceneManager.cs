using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static void LoadScene(int index)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index);
    }
}
