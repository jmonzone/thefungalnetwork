using System.Collections;
using UnityEngine;

public class AutoConnect : MonoBehaviour
{
    [SerializeField] private MultiplayerReference multiplayer;
    [SerializeField] private bool addAIPlayer = false;
    [SerializeField] private GameMode gameMode;

    private IEnumerator Start()
    {
        if (multiplayer.JoinedLobby != null)
        {
            multiplayer.StartHostClient();
        }
        else
        {
            yield return new WaitForSeconds(3f);

            multiplayer.SignIn(() =>
            {
                multiplayer.ListLobbies(async lobbies =>
                {
                    if (lobbies.Count > 0)
                    {
                        var value = await multiplayer.TryJoinLobbyById(lobbies[0].Id);
                        if (value)
                        {
                            var joinCode = multiplayer.JoinedLobby.Data["JoinCode"].Value;
                            multiplayer.JoinRelay(joinCode);
                        }
                    }
                    else
                    {
                        await multiplayer.CreateRelayAndLobby(gameMode);
                        if (addAIPlayer) await multiplayer.AddAIPlayer();
                        multiplayer.StartHostClient();
                    }
                });
            });
        }
    }
}
