using Unity.Netcode;
using UnityEngine;

public class NetworkPowerUp : NetworkBehaviour
{
    [SerializeField] private PowerUpCollection powerUpCollection;

    private PowerUp powerUp;

    private NetworkVariable<int> assignedIndex = new NetworkVariable<int>(-1);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        powerUp = GetComponent<PowerUp>();
        powerUp.HandleCollection = fungal => HandleCollectionServerRpc(fungal.Id);
        powerUp.HandleRespawn = HandleRespawnServerRpc;

        // When assignedIndex is updated (on join or runtime), assign the ability
        assignedIndex.OnValueChanged += (_, newIndex) =>
        {
            if (newIndex >= 0)
            {
                AssignAbility(newIndex);
            }
        };

        // If the value was already set before OnValueChanged was added, apply it immediately
        if (assignedIndex.Value >= 0)
        {
            AssignAbility(assignedIndex.Value);
        }
    }

    [ServerRpc]
    public void AssignAbilityServerRpc(int index)
    {
        Debug.Log("AssignAbilityServerRpc");

        // Set the networked value so all clients (including late joiners) get it
        assignedIndex.Value = index;
    }

    private void AssignAbility(int index)
    {
        Debug.Log("AssignAbility");
        powerUp.AssignAbility(powerUpCollection.PowerUps[index]);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleCollectionServerRpc(ulong fungalId)
    {
        HandleCollectionClientRpc(fungalId);
    }

    [ClientRpc]
    private void HandleCollectionClientRpc(ulong fungalId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(fungalId, out var networkObject))
        {
            var fungal = networkObject.GetComponent<FungalController>();
            powerUp.ApplyCollectLogic(fungal);
            if (IsOwner) powerUp.StartRespawn();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void HandleRespawnServerRpc()
    {
        HandleRespawnClientRpc();
    }

    [ClientRpc]
    private void HandleRespawnClientRpc()
    {
        powerUp.ApplyRespawn();
    }
}
