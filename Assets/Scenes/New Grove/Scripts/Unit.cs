using UnityEngine;

[CreateAssetMenu]
public class Unit : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject prefab;

    public string Name => name;
    public Sprite Sprite => sprite;
    public GameObject Prefab => prefab;
}
