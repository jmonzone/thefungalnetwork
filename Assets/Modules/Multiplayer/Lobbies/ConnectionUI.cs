using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConnectionUI : MonoBehaviour
{
    [SerializeField] private Button createButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private LobbyListUI lobbyListUI;

    public event UnityAction OnCreateButtonClicked;
    public event UnityAction<Lobby> OnLobbyJoinButtonClicked;

    private void Awake()
    {
        exitButton.onClick.AddListener(() => SceneManager.LoadScene("Grove"));
        createButton.onClick.AddListener(() => OnCreateButtonClicked?.Invoke());

        lobbyListUI.OnLobbyJoinButtonClicked += lobby => OnLobbyJoinButtonClicked(lobby);
    }

    public void SetLobbies(List<Lobby> lobbies)
    {
        lobbyListUI.SetLobbies(lobbies);
    }
}
