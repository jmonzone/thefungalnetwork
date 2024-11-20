using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject itemPrefab;
    public Sprite Sprite => sprite;
    public GameObject ItemPrefab => itemPrefab;
}
