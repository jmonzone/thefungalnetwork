using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuMatchmaking : MonoBehaviour
{
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private ConnectionUI connectionUI;
    [SerializeField] private DisplayName displayName;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference partyPrepareView;

    [SerializeField] private Button createPartyButton;

    private void Awake()
    {
        createPartyButton.onClick.AddListener(async () =>
        {
            await multiplayer.CreateLobby();
            navigation.Navigate(partyPrepareView);
        });
    }

    private void Start()
    {
        //todo: should be handled by a global component
        multiplayer.SignIn(displayName.Value, () => StartCoroutine(RefreshLobbyList()));
    }

    private IEnumerator RefreshLobbyList()
    {
        while (gameObject)
        {
            UpdateLobbyList();
            yield return new WaitForSeconds(2f);
        }
    }

    private void UpdateLobbyList()
    {
        multiplayer.ListLobbies(lobbies =>
        {
            connectionUI.SetLobbies(lobbies);
        });
    }
}
