using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//todo: centralize  
public class LeaveSessionButton : MonoBehaviour
{
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private Button button;
    [SerializeField] private SceneNavigation sceneNavigation;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            if (multiplayerManager)
            {
                multiplayerManager.LeaveLobby();
                multiplayerManager.DisconnectRelay();
                sceneNavigation.NavigateToScene(0);
            }
        });
    }
}
