using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;

public class LobbyListUI : MonoBehaviour
{
    [SerializeField] private Transform lobbyItemAnchor;
    [SerializeField] private LobbyItemUI lobbyItemPrefab;

    private List<LobbyItemUI> lobbyItems = new List<LobbyItemUI>();

    public event UnityAction<Lobby> OnLobbyJoinButtonClicked;

    private void Awake()
    {
        lobbyItemAnchor.GetComponentsInChildren(includeInactive: true, lobbyItems);

        foreach(var lobbyItem in lobbyItems)
        {
            lobbyItem.OnJoinButtonClicked += lobby => OnLobbyJoinButtonClicked(lobby);
            lobbyItem.gameObject.SetActive(false);
        }
    }

    public void SetLobbies(List<Lobby> lobbies)
    {
        int existingCount = lobbyItems.Count;
        int requiredCount = lobbies.Count;

        // Instantiate only if needed
        if (existingCount < requiredCount)
        {
            for (int i = existingCount; i < requiredCount; i++)
            {
                LobbyItemUI lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemAnchor);
                lobbyItem.OnJoinButtonClicked += lobby => OnLobbyJoinButtonClicked(lobby);
                lobbyItems.Add(lobbyItem);
            }
        }

        // Update active items and hide the rest
        for (int i = 0; i < lobbyItems.Count; i++)
        {
            if (i < requiredCount)
            {
                lobbyItems[i].SetLobby(lobbies[i]); // Update lobby data
            }
            else
            {
                lobbyItems[i].gameObject.SetActive(false);
            }
        }
    }
}
