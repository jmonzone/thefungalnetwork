using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AutoConnect : MonoBehaviour
{
    [Header("Services")]
    [SerializeField] private MultiplayerReference multiplayer;
    [SerializeField] private FungalCollection fungalCollection;

    [Header("Battle Options")]
    [SerializeField] private GameMode gameMode;
    [SerializeField] private FungalData startingFungal;
    [SerializeField] private List<FungalData> botPlayers;

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
                        bool joined = false;

                        while (!joined)
                        {
                            var value = await multiplayer.TryJoinLobbyById(lobbies[0].Id);

                            if (value)
                            {
                                var joinCode = multiplayer.JoinedLobby.Data["JoinCode"].Value;

                                Debug.Log($"joining {joinCode}");

                                await multiplayer.JoinRelay(joinCode);

                                multiplayer.StartHostClient();
                                joined = true;
                            }
                            else
                            {
                                Debug.Log("Join attempt failed, retrying in 1 second...");
                                await Task.Delay(2000);
                            }
                        }
                    }
                    else
                    {
                        var index = fungalCollection.Fungals.IndexOf(startingFungal);
                        await multiplayer.CreateRelayAndLobby(gameMode, index);

                        foreach (var botFungal in botPlayers)
                        {
                            index = fungalCollection.Fungals.IndexOf(botFungal);
                            await multiplayer.AddAIPlayer(index);
                        }

                        multiplayer.StartHostClient();
                    }
                });
            });
        }
    }

}
