using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Services.Authentication;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MultiplayerTest : MonoBehaviour
{
    [Header("Connection UI References")]
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private GameObject connectUI;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Button createButton;
    [SerializeField] private TMP_InputField lobbyCodeInput;
    [SerializeField] private Button joinButton;

    [Header("Gameplay References")]
    [SerializeField] private PlayerController playerController;

    [Header("Gameplay UI References")]
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private TextMeshProUGUI playersText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private Transform player;

    private bool CanJoin => usernameInput.text.Length > 0 && lobbyCodeInput.text.Length == 6;

    private List<string> firstNames = new List<string> { "Konan", "Zayleen", "Danti", "Stony", "Feni" };
    private List<string> lastNames = new List<string> { "Zonzo", "Varden", "Vunza", "Starita", "Bagnay" };

    // Method to generate a random name
    public string GenerateRandomName()
    {
        string firstName = firstNames[Random.Range(0, firstNames.Count)];
        string lastName = lastNames[Random.Range(0, lastNames.Count)];
        return $"{firstName} {lastName}";
    }

    private void Start()
    {
        connectUI.SetActive(true);
        gameplayUI.SetActive(false);

        createButton.onClick.AddListener(() => Connect(CreateGame));
        joinButton.onClick.AddListener(() => Connect(JoinGame));
        joinButton.interactable = CanJoin;

        usernameInput.text = GenerateRandomName();
        usernameInput.onValueChanged.AddListener(value =>
        {
            createButton.interactable = value.Length > 0;
            joinButton.interactable = CanJoin;
        });

        lobbyCodeInput.onValueChanged.AddListener(value =>
        {
            joinButton.interactable = CanJoin;
        });

        multiplayerManager.OnLobbyUpdated += () => UpdateLobbyInfoUI();

        NetworkPlayer.OnLocalPlayerSpawned += player =>
        {
            this.player = player;

            var movementController = player.GetComponent<MovementController>();
            playerController.SetMovementController(movementController);
        };
    }

    private void Connect(UnityAction onComplete)
    {
        multiplayerManager.SignIn(usernameInput.text, onComplete);
        connectUI.SetActive(false);
    }

    private async void CreateGame()
    {
        await multiplayerManager.CreateRelayAndLobby();
        OnGameJoined();
    }

    private async void JoinGame()
    {
        await multiplayerManager.JoinLobbyByCode(lobbyCodeInput.text);
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
