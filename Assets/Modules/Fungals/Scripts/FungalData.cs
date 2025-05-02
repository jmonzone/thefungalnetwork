using UnityEngine;

public enum FungalType
{
    AQUATIC,
    SKY
}

[CreateAssetMenu(menuName = "Fungals/New Fungal Data")]
public class FungalData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] [TextArea] private string description;
    [SerializeField] private FungalType type;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite actionImage;
    [SerializeField] private Color actionColor;
    [SerializeField] private Color eggColor;
    [SerializeField] private Ability ability1;
    [SerializeField] private Ability ability2;

    public string Id => id;
    public string Description => description;
    public FungalType Type => type;
    public GameObject Prefab => prefab;
    public Sprite ActionImage => actionImage;
    public Color ActionColor => actionColor;
    public Color EggColor => eggColor;
    public Ability Ability1 => ability1;
    public Ability Ability2 => ability2;
}