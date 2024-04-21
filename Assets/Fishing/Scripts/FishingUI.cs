using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FishingUI : MonoBehaviour
{
    [SerializeField] private Button backButton;

    private void Start()
    {
        backButton.onClick.AddListener(() => SceneManager.LoadScene(0));
    }
}
