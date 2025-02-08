using Unity.Netcode;
using UnityEngine;

public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FungalInventory fungalInventory;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        arena.RegisterPlayer(transform);

        var fungalController = GetComponent<FungalController>();
        fungalController.InitializeAnimations();
    }
}
