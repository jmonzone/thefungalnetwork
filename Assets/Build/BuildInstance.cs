using UnityEngine;

[CreateAssetMenu]
public class BuildInstance : ScriptableObject
{
    [SerializeField] private Item item;
    [SerializeField] private Vector3 position;
    [SerializeField] private Quaternion rotation;

    public Item Item => item;
    public Vector3 Position => position;
    public Quaternion Rotation => rotation;

    public void Initialize(Item item, Vector3 position, Quaternion rotation)
    {
        this.item = item;
        this.position = position;
        this.rotation = rotation;
    }
}
