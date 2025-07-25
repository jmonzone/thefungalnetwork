using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour
{
    public const int INTRO_SCENE_INDEX = 1;
    public const int MENU_SCENE_INDEX = 2;

    private void Start()
    {
        if (PlayerPrefs.GetInt(IntroSequence.INTRO_COMPLETE_KEY, 0) == 1)
        {
            SceneManager.LoadScene(MENU_SCENE_INDEX);
        }
        else
        {
            SceneManager.LoadScene(INTRO_SCENE_INDEX);
        }
    }
}
