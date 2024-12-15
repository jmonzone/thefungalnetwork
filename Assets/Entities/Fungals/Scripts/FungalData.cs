using Unity.Netcode;
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
    [SerializeField] private FungalType type;
    [SerializeField] private GameObject prefab;
    [SerializeField] private NetworkFungal networkPrefab;
    [SerializeField] private Sprite actionImage;
    [SerializeField] private Color actionColor;
    [SerializeField] private Color eggColor;

    public string Id => id;
    public FungalType Type => type;
    public GameObject Prefab => prefab;
    public NetworkFungal NetworkPrefab => networkPrefab;
    public Sprite ActionImage => actionImage;
    public Color ActionColor => actionColor;
    public Color EggColor => eggColor;

}
