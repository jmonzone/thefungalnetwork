using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaveSessionButton : MonoBehaviour
{
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            multiplayerManager.LeaveLobby();
            multiplayerManager.DisconnectRelay();
            SceneManager.LoadScene(2);
        });
    }
}
