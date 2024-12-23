using Unity.Netcode;
using UnityEngine;


public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private MovementController movement;

    public MovementController Movement => movement;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        arena.RegisterPlayer(transform);

        var fungalController = GetComponent<FungalController>();
        fungalController.InitializeAnimations();
    }

    [ServerRpc]
    public void SetAsMinionServerRpc()
    {
        SetAsMinionClientRpc();
    }

    [ClientRpc]
    private void SetAsMinionClientRpc()
    {
        Debug.Log("I am the minion");

    }
}
