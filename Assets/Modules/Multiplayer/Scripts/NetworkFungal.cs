using Unity.Netcode;
using UnityEngine;


public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private Controllable controllable;

    public Controllable Controllable => controllable;

    public void Initialize(FungalData fungal)
    {
        Instantiate(fungal.Prefab, transform);
    }
}
