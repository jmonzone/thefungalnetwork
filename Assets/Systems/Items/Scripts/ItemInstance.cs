using UnityEngine;
using UnityEngine.Events;

public class ItemInstance : ScriptableObject
{
    [SerializeField] private Item item;
    [SerializeField] private int count;

    public Item Data => item;
    public int Count
    {
        get => count;
        set => count = value;
    }

    public event UnityAction OnConsumed;

    public void Initialize(Item item, int count)
    {
        name = item.name;
        this.item = item;
        this.count = count;
    }

    public void Consume()
    {
        OnConsumed?.Invoke();
    }
}
