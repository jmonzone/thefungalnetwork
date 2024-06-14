using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;

    public string Name => name;
    public Sprite Sprite => sprite;
}


public class ItemInstance : ScriptableObject
{
    [SerializeField] private Item item;

    public Item Data => item;

    public event UnityAction OnConsumed;

    public void Initialize(Item item)
    {
        this.item = item;
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
