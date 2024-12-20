using Unity.Collections;
using Unity.Netcode;
using UnityEngine;


public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private MovementController movement;

    public MovementController Movement => movement;
    public FungalModel Fungal { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();


        arena.RegisterPlayer(transform);

        // Subscribe to the value change event
        Debug.Log("OnNetworkSpawn");

    }
}
