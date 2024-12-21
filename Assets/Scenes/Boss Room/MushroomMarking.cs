using Unity.Netcode;
using UnityEngine;

public class MushroomMarking : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // check if the the collider is the mushroom ball
        var objective = other.GetComponentInParent<MushroomObjective>();
        if (objective)
        {
            Debug.Log("objective found");
            objective.UnmountServerRpc();
        }
    }
}
