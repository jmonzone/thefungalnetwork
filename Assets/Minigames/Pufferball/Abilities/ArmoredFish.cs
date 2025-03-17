using Unity.Netcode;
using UnityEngine;

public class ArmoredFish : MonoBehaviour
{
    [SerializeField] private PufferballReference pufferballReference;
    [SerializeField] private NetworkObject armorPrefab;

    private void Awake()
    {
        var fish = GetComponent<Fish>();
        fish.OnPickup += SpawnArmorServerRpc;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnArmorServerRpc()
    {
        pufferballReference.Player.fungal.AddShieldServerRpc(2f);

        var armor = Instantiate(armorPrefab, pufferballReference.Player.fungal.transform.position, Quaternion.identity);
        armor.Spawn();

        var armorMovement = armor.GetComponent<Movement>();
        armorMovement.Follow(pufferballReference.Player.fungal.transform);
    }
}
