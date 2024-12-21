using Unity.Collections;
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

        if (!IsOwner)
        {
            var proximityAction = GetComponent<ProximityAction>();
            proximityAction.SetInteractable(false);
        }

        var fungalController = GetComponent<FungalController>();
        fungalController.InitializeAnimations();
    }
}
