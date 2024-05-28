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
    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite actionImage;
    [SerializeField] private Color actionColor;
    [SerializeField] private Color eggColor;

    public string Name => name;
    public PetType Type => type;
    public GameObject Prefab => prefab;
    public Sprite ActionImage => actionImage;
    public Color ActionColor => actionColor;
    public Color EggColor => eggColor;

}
