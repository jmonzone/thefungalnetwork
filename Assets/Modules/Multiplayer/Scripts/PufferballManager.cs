using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PufferballManager : MonoBehaviour
{
    [Header("Gameplay References")]
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private InputManager playerController;
    [SerializeField] private PufferballController pufferballPrefab;

    [Header("UI References")]
    [SerializeField] private ConnectionUI connectionUI;
    [SerializeField] private GameObject inputUI;
    [SerializeField] private Button exitButton;

    private void Start()
    {  
        var username = GenerateRandomName();
        multiplayerManager.SignIn(username, () => StartCoroutine(RefreshLobbyList()));

        connectionUI.gameObject.SetActive(true);
        connectionUI.OnCreateButtonClicked += () => CreateGame();
        connectionUI.OnRefreshButtonClicked += () => UpdateLobbyList();
        connectionUI.OnLobbyJoinButtonClicked += lobby => JoinGame(lobby.Id);

        PufferballPlayer.OnLocalPlayerSpawned += player =>
        {
            var movementController = player.GetComponent<MovementController>();
            playerController.SetMovementController(movementController);
        };

        exitButton.onClick.AddListener(() =>
        {
            multiplayerManager.LeaveLobby();
            multiplayerManager.DisconnectRelay();
            StopAllCoroutines();
            Utility.LoadScene("Grove");
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


    private List<string> firstNames = new List<string> { "Konan", "Zayleen", "Danti", "Stony", "Feni" };
    private List<string> lastNames = new List<string> { "Zonzo", "Varden", "Vunza", "Starita", "Bagnay" };



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
