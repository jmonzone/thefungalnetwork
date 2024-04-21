using UnityEngine;

public enum PetType
{
    AQUATIC,
    SKY
}

[CreateAssetMenu]
public class Pet : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private PetType type;
    [SerializeField] private Color color;
    [SerializeField] private GameObject prefab;

    public string Name => name;
    public PetType Type => type;
    public Color Color => color;
    public GameObject Prefab => prefab;
}
