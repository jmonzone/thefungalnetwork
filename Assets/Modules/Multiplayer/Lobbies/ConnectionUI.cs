using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConnectionUI : MonoBehaviour
{
    [SerializeField] private Button createButton;
    [SerializeField] private Button refreshButton;

    [SerializeField] private ListUI lobbyListUI;

    public event UnityAction OnCreateButtonClicked;
    public event UnityAction OnRefreshButtonClicked;
    public event UnityAction<Lobby> OnLobbyJoinButtonClicked;

    private void Awake()
    {
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
    }

    public void SetLobbies(List<Lobby> lobbies)
    {
        var lobbyListData = lobbies.Select(lobby => new ListItemData
        {
            label = lobby.Data["HostName"].Value.Replace("_", " "),
            onClick = () => OnLobbyJoinButtonClicked?.Invoke(lobby),
        }).ToList();

        lobbyListUI.SetItems(lobbyListData);
    }
}
