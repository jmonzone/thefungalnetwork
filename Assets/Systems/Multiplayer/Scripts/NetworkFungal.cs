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
    }

    public void Initialize(string name)
    {
        Fungal = fungalInventory.Fungals.Find(fungal => fungal.Data.name == name);

        //todo: centralize with FungalController
        movement.SetMaxJumpCount(Fungal.Data.Type == FungalType.SKY ? 2 : 1);
    }
}
