using UnityEngine;

[CreateAssetMenu]
public class FishData : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private FishController prefab;
    [SerializeField] private float experience;

    public Sprite Sprite => sprite;
    public FishController Prefab => prefab;
    public float Experience => experience;
}
