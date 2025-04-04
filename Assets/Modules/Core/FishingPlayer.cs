using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class FishingPlayer : NetworkBehaviour
{
    [SerializeField] private DisplayName displayName;
    [SerializeField] private MultiplayerReference multiplayer;

}
