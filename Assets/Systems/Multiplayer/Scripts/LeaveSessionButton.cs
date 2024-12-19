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
        button.onClick.AddListener(() =>
        {
            multiplayer.RequestDisconnect();
        });
    }

    private void OnEnable()
    {
        multiplayer.OnRelayDisconnect += Multiplayer_OnRelayDisconnect;
    }

    private void OnDisable()
    {
        multiplayer.OnRelayDisconnect -= Multiplayer_OnRelayDisconnect;
    }

    private void Multiplayer_OnRelayDisconnect()
    {
        sceneNavigation.NavigateToScene(0);
    }
}
