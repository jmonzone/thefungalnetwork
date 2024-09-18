using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConnectionUI : MonoBehaviour
{
    [SerializeField] private Button createButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button refreshButton;

    [SerializeField] private LobbyListUI lobbyListUI;

    public event UnityAction OnCreateButtonClicked;
    public event UnityAction OnRefreshButtonClicked;
    public event UnityAction<Lobby> OnLobbyJoinButtonClicked;

    private void Awake()
    {
        exitButton.onClick.AddListener(() => SceneManager.LoadScene("Grove"));
        createButton.onClick.AddListener(() => OnCreateButtonClicked?.Invoke());
        refreshButton.onClick.AddListener(() =>
        {
            refreshButton.interactable = false;
            OnRefreshButtonClicked?.Invoke();

            IEnumerator RefreshButton()
            {
                yield return new WaitForSeconds(1f);
                refreshButton.interactable = true;
            }

            StartCoroutine(RefreshButton());
        });
        lobbyListUI.OnLobbyJoinButtonClicked += lobby => OnLobbyJoinButtonClicked(lobby);
    }

    public void SetLobbies(List<Lobby> lobbies)
    {
        lobbyListUI.SetLobbies(lobbies);
    }
}
