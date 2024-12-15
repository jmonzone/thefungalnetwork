using System.Collections;
using UnityEngine;

public class MainMenuMatchmaking : MonoBehaviour
{
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private ConnectionUI connectionUI;
    [SerializeField] private DisplayName displayName;
    [SerializeField] private ViewReference viewReference;

    private void FadeIn()
    {
        multiplayerManager.SignIn(displayName.name, () => StartCoroutine(RefreshLobbyList()));
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
}
