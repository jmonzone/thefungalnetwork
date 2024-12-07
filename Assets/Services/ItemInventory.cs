using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class ItemInventory : ScriptableObject
{
    [SerializeField] private LocalData localData;
    [SerializeField] private GameData gameData;
    [SerializeField] private List<ItemInstance> items;

    public List<ItemInstance> Items => items.Where(item => item.Count > 0).ToList();

    public event UnityAction OnInventoryUpdated;

    public event UnityAction<ItemInstance> OnItemAdded;

    private const string INVENTORY_KEY = "inventory";

    public int GetItemCount(Item item) => items.FirstOrDefault(i => i.Data == item)?.Count ?? 0;

    public void Initialize()
    {
        items = new List<ItemInstance>();

        if (localData.JsonFile.ContainsKey(INVENTORY_KEY))
        {
            foreach (var item in localData.JsonFile[INVENTORY_KEY] as JArray)
            {
                if (item is JObject itemJson)
                {
                    var itemData = gameData.Items.Find(item => item.name == itemJson["name"].ToString());
                    if (itemData) AddToInventory(itemData, (int)itemJson["count"]);
                    else Debug.LogWarning($"Item {itemJson} not found in game data");
                };
            }
        }
    }

    private void SaveData()
    {
        var inventoryJson = new JArray();

        foreach (var item in Items)
        {
            inventoryJson.Add(new JObject
            {
                ["name"] = item.Data.name,
                ["count"] = item.Count,
            });
        }

        localData.SaveData(INVENTORY_KEY, inventoryJson);
    }

    public void AddToInventory(Item item, int amount)
    {
        //Debug.Log($"adding item {item.name} {amount}");

        // Find if the item already exists in the inventory
        var existingItem = items.FirstOrDefault(i => i.Data.name == item.name);
        if (existingItem != null)
        {
            // Update the count for the existing item
            existingItem.Count += amount;
            OnItemAdded?.Invoke(existingItem);
            //Debug.Log($"incrementing item {item.name} count to {existingItem.Count}");
        }
        else
        {
            // Add a new item if it doesn't exist
            //Debug.Log($"adding new item {item.name}");
            var targetItem = CreateInstance<ItemInstance>();
            targetItem.Initialize(item, amount);
            items.Add(targetItem);
            OnItemAdded?.Invoke(targetItem);
        }

        SaveData();
        OnInventoryUpdated?.Invoke();
    }

    public void RemoveFromInventory(Item item, int amount)
    {
        var existingItem = items.FirstOrDefault(i => i.Data.name == item.name);
        if (existingItem && existingItem.Count >= amount)
        {
            existingItem.Count -= amount;
            SaveData();
            OnInventoryUpdated?.Invoke();
        }
        else
        {
            Debug.LogWarning("item does not exist in inventory");
        }
    }
}
