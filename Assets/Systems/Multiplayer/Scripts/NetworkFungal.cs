using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private Controller controller;
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
}
