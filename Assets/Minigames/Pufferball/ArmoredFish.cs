using Unity.Netcode;
using UnityEngine;

public class ArmoredFish : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private NetworkObject armorPrefab;

    private void Awake()
    {
        var fish = GetComponent<Fish>();
        fish.OnPickup += SpawnArmorServerRpc;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnArmorServerRpc()
    {
        var armor = Instantiate(armorPrefab, playerReference.Movement.transform.position, Quaternion.identity);
        armor.Spawn();

        var armorMovement = armor.GetComponent<Movement>();
        armorMovement.Follow(playerReference.Movement.transform);

    }
}
