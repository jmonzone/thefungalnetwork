using System.Collections;
using UnityEngine;

public class MainMenuMatchmaking : MonoBehaviour
{
    [SerializeField] private FadeCanvasGroup canvasGroup;
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private ConnectionUI connectionUI;
    [SerializeField] private DisplayName displayName;

    private void Awake()
    {
        canvasGroup.gameObject.SetActive(false);
    }


    public IEnumerator FadeIn()
    {
        multiplayerManager.SignIn(displayName.name, () => StartCoroutine(RefreshLobbyList()));

        yield return canvasGroup.FadeIn();
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
