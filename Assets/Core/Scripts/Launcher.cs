using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour
{
    [SerializeField] private Tutorial tutorial;

    private void Start()
    {
        if (tutorial.IsCompleted) SceneManager.LoadScene(2);
        else SceneManager.LoadScene(1);
    }
}
