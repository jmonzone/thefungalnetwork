using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PufferballManager : MonoBehaviour
{
    [Header("Arena References")]
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private Transform spawnAnchor1;
    [SerializeField] private Transform spawnAnchor2;
    [SerializeField] private Transform cameraController;

    [Header("Gameplay References")]
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PufferballController pufferballPrefab;

    [Header("UI References")]
    [SerializeField] private ConnectionUI connectionUI;
    [SerializeField] private GameObject inputUI;
    [SerializeField] private Button exitButton;

    private PufferballPlayer player;

    private List<string> firstNames = new List<string> { "Hollow", "Sharon", "Jesus", "Charmander", "Feni" };
    private List<string> lastNames = new List<string> { "Zozo", "Venga", "Lotus", "Atari", "Bagus" };

    private void Awake()
    {
        arena.Initialize(spawnAnchor1.position, spawnAnchor2.position);
    }

    private void Start()
    {  
        var username = GenerateRandomName();
        //multiplayerManager.SignIn(username, () => StartCoroutine(RefreshLobbyList()));
        multiplayerManager.SignIn(username, () => StartCoroutine(AutoJoinBogRoom()));

        connectionUI.gameObject.SetActive(true);
        connectionUI.OnCreateButtonClicked += () => CreateGame();
        connectionUI.OnRefreshButtonClicked += () => UpdateLobbyList();
        connectionUI.OnLobbyJoinButtonClicked += lobby => JoinGame(lobby.Id);

        PufferballPlayer.OnLocalPlayerSpawned += player =>
        {
            this.player = player;
            inputManager.SetControllable(player);
            inputManager.OnInteractionButtonClicked += () => player.LaunchBall();

            var targetRotation = new Vector3(0, this.player.IsHost ? 45 : 45 + 180, 0);
            cameraController.transform.eulerAngles = targetRotation;
        };

        exitButton.onClick.AddListener(() =>
        {
            multiplayerManager.LeaveLobby();
            multiplayerManager.DisconnectRelay();
            StopAllCoroutines();
            Utility.LoadScene("Grove");
        });
    }


    private void Update()
    {
        if (player) inputManager.CanInteract(player.HasPufferball);
    }

    private IEnumerator AutoJoinBogRoom()
    {
        yield return new WaitForSeconds(2f);

        multiplayerManager.ListLobbies(lobbies =>
        {
            Debug.Log(lobbies.Count);
            if (lobbies.Count > 0)
            {
                JoinGame(lobbies[0].Id);
            }
            else
            {
                CreateGame();
            }
        });
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
        multiplayerManager.ListLobbies(lobbies =>
        {
            connectionUI.SetLobbies(lobbies);
        });
    }

    // Method to generate a random name
    public string GenerateRandomName()
    {
        string firstName = firstNames[Random.Range(0, firstNames.Count)];
        string lastName = lastNames[Random.Range(0, lastNames.Count)];
        return $"{firstName} {lastName}";
    }

    private async void CreateGame()
    {
        connectionUI.gameObject.SetActive(false);
        await multiplayerManager.CreateRelayAndLobby();
        OnGameJoined();
    }

    private async void JoinGame(string id)
    {
        connectionUI.gameObject.SetActive(false);
        await multiplayerManager.JoinLobbyById(id);
        OnGameJoined();
    }

    private void OnGameJoined()
    {
        inputUI.SetActive(true);
    }
}
