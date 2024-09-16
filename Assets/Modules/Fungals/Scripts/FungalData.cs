using UnityEngine;

public enum FungalType
{
    AQUATIC,
    SKY
}

[CreateAssetMenu]
public class FungalData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private FungalType type;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite actionImage;
    [SerializeField] private Color actionColor;
    [SerializeField] private Color eggColor;

    public string Id => id;
    public FungalType Type => type;
    public GameObject Prefab => prefab;
    public Sprite ActionImage => actionImage;
    public Color ActionColor => actionColor;
    public Color EggColor => eggColor;

}
