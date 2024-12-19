using System.Collections;
using System.Linq;
using GURU;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuMatchmaking : MonoBehaviour
{
    [SerializeField] private MultiplayerManager multiplayer;
    [SerializeField] private DisplayName displayName;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference partyPrepareView;

    [SerializeField] private Button createPartyButton;
    [SerializeField] private ListUI lobbyListUI;

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
            var lobbyListData = lobbies.Select(lobby => new ListItemData
            {
                label = lobby.Data["HostName"].Value.Replace("_", " "),
                onClick = async () =>
                {
                    await MultiplayerManager.Instance.JoinLobbyById(lobby.Id);
                    navigation.Navigate(partyPrepareView);
                }
            }).ToList();

            lobbyListUI.SetItems(lobbyListData);
        });

    }
}
