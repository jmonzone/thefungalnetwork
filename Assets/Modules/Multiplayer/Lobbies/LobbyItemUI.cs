using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hostNameText;
    [SerializeField] private Button joinButton;

    public Lobby Lobby { get; private set; }
    public event UnityAction<Lobby> OnJoinButtonClicked;

    private void Awake()
    {
        joinButton.onClick.AddListener(() => OnJoinButtonClicked?.Invoke(Lobby));
    }

    public void SetLobby(Lobby lobby)
    {
        Lobby = lobby;
        hostNameText.text = lobby.Data["HostName"].Value.Replace("_", " ");
        gameObject.SetActive(true);
    }
}
