using UnityEngine;

[CreateAssetMenu]
public class FishData : Item
{
    [SerializeField] private FishController prefab;
    [SerializeField] private float experience;

    public FishController Prefab => prefab;
    public float Experience => experience;
}
