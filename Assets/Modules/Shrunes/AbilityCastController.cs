using UnityEngine;

// this scripts is used to handle ability casts events in a scene
public class AbilityCastController : MonoBehaviour
{
    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private ShruneCollection shruneCollection;
    [SerializeField] private ItemInventory itemInventory;

    private void OnEnable()
    {
        itemInventory.OnInventoryUpdated += ItemInventory_OnInventoryUpdated;
        abilityCast.OnComplete += CastAbility;
    }

    private void OnDisable()
    {
        itemInventory.OnInventoryUpdated -= ItemInventory_OnInventoryUpdated;
        abilityCast.OnComplete -= CastAbility;
    }

    private void ItemInventory_OnInventoryUpdated()
    {
        if (!abilityCast.Shrune)
        {
            var shrune = itemInventory.Items.Find(item => item.Data is ShruneItem);
            if (shrune) abilityCast.SetShrune(shrune.Data as ShruneItem);
        }
    }

    private void CastAbility()
    {
        if (shruneCollection.TryGetShruneById(abilityCast.ShruneId, out ShruneItem shrune))
        {
            var projectile = Instantiate(shrune.ProjectilePrefab, abilityCast.StartPosition, Quaternion.LookRotation(abilityCast.Direction), transform);
            projectile.Shoot(abilityCast.Direction, abilityCast.MaxDistance);
        }
    }
}
