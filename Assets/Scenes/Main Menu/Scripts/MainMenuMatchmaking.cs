using System.Linq;
using GURU;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuMatchmaking : MonoBehaviour
{
    [SerializeField] private MultiplayerReference multiplayer;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference partyPrepareView;

    [SerializeField] private Button createPartyButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI noPartiesText;
    [SerializeField] private ListUI lobbyListUI;
    [SerializeField] private FadeCanvasGroup errorMessage;

    private void Awake()
    {
        createPartyButton.onClick.AddListener(async () =>
        {
            await multiplayer.CreateLobby();
            navigation.Navigate(partyPrepareView);
        });

        exitButton.onClick.AddListener(() =>
        {
            enabled = false;
            //await multiplayer.LeaveLobby();
            navigation.GoBack();
        });

        GetComponent<ViewController>().OnFadeInStart += () =>
        {
            if (!multiplayer.IsSignedIn)
            {
                multiplayer.SignIn(() =>
                {
                    UpdateLobbyList();
                    enabled = true;
                });
            }
            else
            {
                enabled = true;
            }
        };
    }

    private float updateTimer = 0f;

    private void Update()
    {
        if (!multiplayer.IsSignedIn) return;
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
                    if (await multiplayer.TryJoinLobbyById(lobby.Id))
                    {
                        navigation.Navigate(partyPrepareView);
                    }
                    else
                    {
                        StartCoroutine(errorMessage.FadeIn());
                    }
                }
            }).ToList();

            lobbyListUI.SetItems(lobbyListData);
            noPartiesText.gameObject.SetActive(lobbyListData.Count == 0);
        });

    }
}
