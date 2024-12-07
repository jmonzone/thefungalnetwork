using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button groveButton;

    public event UnityAction OnTutorialButtonClicked;

    private void Awake()
    {
        tutorialButton.onClick.AddListener(() => OnTutorialButtonClicked?.Invoke());
        groveButton.onClick.AddListener(() => SceneManager.LoadScene(2));
    }
}
