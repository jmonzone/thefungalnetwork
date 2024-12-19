using UnityEngine;
using UnityEngine.UI;

//todo: centralize  
public class LeaveSessionButton : MonoBehaviour
{
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private Button button;
    [SerializeField] private SceneNavigation sceneNavigation;

    private void Awake()
    {
        button.onClick.AddListener(async() =>
        {
            multiplayer.DisconnectFromRelay();
            await multiplayer.RemoveRelayFromLobbyData();
            sceneNavigation.NavigateToScene(0);
        });
    }
}
