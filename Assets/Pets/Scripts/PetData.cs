using UnityEngine;

[CreateAssetMenu]
public class PetData : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Color color;
    [SerializeField] private GameObject prefab;

    public string Name => name;
    public Color Color => color;
    public GameObject Prefab => prefab;
}
