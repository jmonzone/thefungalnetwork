using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class AutoConnect : MonoBehaviour
{
    [SerializeField] private MultiplayerReference multiplayer;
    [SerializeField] private bool addAIPlayer = false;
    [SerializeField] private GameMode gameMode;

    private void Start()
    {
        if (multiplayer.JoinedLobby != null)
        {
            //Debug.Log($"AutoConnect multiplayer.IsHost: {multiplayer.IsHost}");
            var value = multiplayer.GetJoinedLobbyData("UseAI");
            if (string.IsNullOrEmpty(value)) value = "true";
            //Debug.Log($"AutoConnect multiplayer.value: {value}");

            if (multiplayer.IsHost && bool.Parse(value))
            {
                //Debug.Log($"AutoConnect multiplayer.useAI");
                //await multiplayer.AddAIPlayer();
            }

            multiplayer.StartHostClient();
        }
        else
        {
            StartCoroutine(AutoConnectRoutine());
        }
    }

    private IEnumerator AutoConnectRoutine()
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
                    //if (addAIPlayer) await multiplayer.AddAIPlayer();
                    multiplayer.StartHostClient();
                }
            });
        });
    }

}
