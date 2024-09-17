using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MultiplayerTest : MonoBehaviour
{
    [Header("Connection UI References")]
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private ConnectionUI connectionUI;

    [Header("Gameplay References")]
    [SerializeField] private PlayerController playerController;

    [Header("Gameplay UI References")]
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private TextMeshProUGUI playersText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Start()
    {
        connectionUI.gameObject.SetActive(true);
        connectionUI.OnCreateButtonClicked += () => Connect(CreateGame);
        connectionUI.OnJoinButtonClicked += code => Connect(() => JoinGame(code));

        gameplayUI.SetActive(false);

        multiplayerManager.OnLobbyUpdated += () => UpdateLobbyInfoUI();

        NetworkPlayer.OnLocalPlayerSpawned += player =>
        {
            var movementController = player.GetComponent<MovementController>();
            playerController.SetMovementController(movementController);
        };
    }


    private List<string> firstNames = new List<string> { "Konan", "Zayleen", "Danti", "Stony", "Feni" };
    private List<string> lastNames = new List<string> { "Zonzo", "Varden", "Vunza", "Starita", "Bagnay" };



    // Method to generate a random name
    public string GenerateRandomName()
    {
        string firstName = firstNames[Random.Range(0, firstNames.Count)];
        string lastName = lastNames[Random.Range(0, lastNames.Count)];
        return $"{firstName} {lastName}";
    }

    private void Connect(UnityAction onComplete)
    {
        var username = GenerateRandomName();
        multiplayerManager.SignIn(username, onComplete);
        connectionUI.gameObject.SetActive(false);
    }

    private async void CreateGame()
    {
        await multiplayerManager.CreateRelayAndLobby();
        OnGameJoined();
    }

    private async void JoinGame(string code)
    {
        await multiplayerManager.JoinLobbyByCode(code);
        OnGameJoined();
    }

    private void OnGameJoined()
    {
        gameplayUI.SetActive(true);
        UpdateLobbyInfoUI();
    }

    private void UpdateLobbyInfoUI()
    {
        lobbyCodeText.text = multiplayerManager.JoinedLobby.LobbyCode;

        playersText.text = "<b>Players:</b> ";

        foreach (var player in multiplayerManager.JoinedLobby.Players)
        {
            playersText.text += player.Data["PlayerName"].Value.Replace("_", " ");
            if (AuthenticationService.Instance.PlayerId == player.Id) playersText.text += " (You)";

            playersText.text += " ";

        }
    }
}
