using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoConnect : MonoBehaviour
{
    [SerializeField] private MultiplayerReference multiplayer;
    [SerializeField] private List<FungalData> botPlayers;
    [SerializeField] private FungalCollection fungalCollection;
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
                        foreach(var botFungal in botPlayers)
                        {
                            var index = fungalCollection.Fungals.IndexOf(botFungal);
                            await multiplayer.AddAIPlayer(index);
                        }

                        multiplayer.StartHostClient();
                    }
                });
            });
        }
    }
}
