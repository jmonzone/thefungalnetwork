using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button groveButton;

    private void Awake()
    {
        tutorialButton.onClick.AddListener(() => SceneManager.LoadScene(1));
        groveButton.onClick.AddListener(() => SceneManager.LoadScene(2));
    }
}
