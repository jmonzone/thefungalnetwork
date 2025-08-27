using UnityEngine;

public class BuildInstance : ScriptableObject
{
    [SerializeField] private Item item;
    [SerializeField] private Vector3 position;

    public Item Item => item;
    public Vector3 Position => position;

    public void Initialize(Item item, Vector3 position)
    {
        this.item = item;
        this.position = position;
    }
}
