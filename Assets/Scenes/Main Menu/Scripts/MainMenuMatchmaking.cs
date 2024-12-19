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

    private float updateTimer = 0f;

    private void Update()
    {
        updateTimer += Time.deltaTime;

        if (updateTimer > 2)
        {
            UpdateLobbyList();
            updateTimer = 0;
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
                    await multiplayer.JoinLobbyById(lobby.Id);
                    navigation.Navigate(partyPrepareView);
                }
            }).ToList();

            lobbyListUI.SetItems(lobbyListData);
        });

    }
}
