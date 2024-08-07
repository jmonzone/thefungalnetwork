using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MultiplayerTest : MonoBehaviour
{
    [Header("Game References")]
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    [Header("Connect References")]
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private GameObject connectUI;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Button createButton;

    [Header("Join References")]
    [SerializeField] private TMP_InputField lobbyCodeInput;
    [SerializeField] private Button joinButton;

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


        virtualJoystick.gameObject.SetActive(false);
        virtualJoystick.OnJoystickUpdate += direction =>
        {
            if (!player) return;
            var mappedDirection = new Vector3(direction.x, 0, direction.y);
            player.transform.position += mappedDirection;
        };

        NetworkPlayer.OnLocalPlayerSpawned += player =>
        {
            this.player = player;
            cameraController.Target = player;
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
        lobbyCodeText.text = multiplayerManager.JoinedLobby.LobbyCode;
        virtualJoystick.gameObject.SetActive(true);
    }
}
