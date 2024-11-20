using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;
}


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
        this.item = item;
        this.count = count;
    }

    public void Consume()
    {
        OnConsumed?.Invoke();
    }
}

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    [SerializeField] private List<FungalData> fungals;
    [SerializeField] private List<Item> items;

    public List<FungalData> Fungals => fungals;
    public List<Item> Items => items;

}
