using Unity.Netcode;
using UnityEngine;


public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private Controllable controllable;

    public Controllable Controllable => controllable;
    public FungalModel Fungal { get; private set; }

    public void Initialize(string name)
    {
        Fungal = fungalInventory.Fungals.Find(fungal => fungal.Data.name == name);

        //todo: centralize with FungalController
        var movement = GetComponent<MovementController>();
        movement.SetMaxJumpCount(Fungal.Data.Type == FungalType.SKY ? 2 : 1);
    }
}
