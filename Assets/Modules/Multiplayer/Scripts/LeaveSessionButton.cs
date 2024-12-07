using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaveSessionButton : MonoBehaviour
{
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private Button button;
    [SerializeField] private SceneNavigation sceneNavigation;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            multiplayerManager.LeaveLobby();
            multiplayerManager.DisconnectRelay();
            sceneNavigation.NavigateToScene(2);
        });
    }
}
