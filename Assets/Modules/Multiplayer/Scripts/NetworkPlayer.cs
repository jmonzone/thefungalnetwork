using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkPlayer : NetworkBehaviour
{
    public static UnityAction<Transform> OnLocalPlayerSpawned;
    public static UnityAction<Transform> OnRemotePlayerSpawned;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            // This is the local player
            Debug.Log("Local player spawned: " + gameObject.name);
            OnLocalPlayerSpawned?.Invoke(transform);
        }
        else
        {
            // This is a remote player
            Debug.Log("Remote player spawned: " + gameObject.name);
            OnRemotePlayerSpawned?.Invoke(transform);
        }
    }
}
