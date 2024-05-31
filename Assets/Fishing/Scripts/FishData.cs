using UnityEngine;

[CreateAssetMenu]
public class FishData : Item
{
    [SerializeField] private FishController prefab;
    [SerializeField] private float experience;
    [SerializeField] private int levelRequirement;

    public FishController Prefab => prefab;
    public float Experience => experience;
    public int LevelRequirement => levelRequirement;
}
