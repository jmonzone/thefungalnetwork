using Unity.Netcode;
using UnityEngine;

public class MushroomMarking : NetworkBehaviour
{
    [SerializeField] private MultiplayerArena multiplayerArena;

    private void OnTriggerEnter(Collider other)
    {
        // check if the the collider is the mushroom ball
        var objective = other.GetComponentInParent<Mountable>();
        if (objective && objective.IsMounted.Value)
        {
            if (IsOwner)
            {
                objective.Unmount();
                OnMushroomCollectedClientRpc();
            }
        }
    }

    [ClientRpc]
    public void OnMushroomCollectedClientRpc()
    {
        multiplayerArena.IncrementMushroomsCollected();
    }

}
