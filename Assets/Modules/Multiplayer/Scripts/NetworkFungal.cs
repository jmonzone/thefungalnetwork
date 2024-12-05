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
        Debug.Log(name);
        Fungal = fungalInventory.Fungals.Find(fungal => fungal.Data.name == name);
        Debug.Log(Fungal);
        if (Fungal) Instantiate(Fungal.Data.Prefab, transform);
    }
}
