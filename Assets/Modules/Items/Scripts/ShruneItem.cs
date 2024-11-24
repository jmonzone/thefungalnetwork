using UnityEngine;

[CreateAssetMenu]
public class ShruneItem : Item
{
    [SerializeField] private MovementController spellPrefab;

    public MovementController SpellPrefab => spellPrefab;
}
