using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class InventoryService : ScriptableObject
{
    [SerializeField] private GameData gameData;
    [SerializeField] private List<ItemInstance> inventory;
    public List<ItemInstance> Inventory => inventory;

    public event UnityAction OnInventoryOpened;
    public event UnityAction OnInventoryClosed;
    public event UnityAction OnInventoryUpdated;

    public event UnityAction<ItemInstance> OnItemAdded;

    private const string INVENTORY_KEY = "inventory";

    public void OpenInventory()
    {
        OnInventoryOpened?.Invoke();
    }

    public void CloseInventory()
    {
        OnInventoryClosed?.Invoke();
    }

    public int GetItemCount(Item item) => inventory.FirstOrDefault(i => i.Data == item)?.Count ?? 0;

    public void Initialize(JObject jsonFile)
    {
        inventory = new List<ItemInstance>();

        if (jsonFile.ContainsKey(INVENTORY_KEY))
        {
            foreach (var item in jsonFile[INVENTORY_KEY] as JArray)
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

    public void SaveData(JObject jsonFile)
    {
        var inventoryJson = new JArray();

        foreach (var item in Inventory)
        {
            inventoryJson.Add(new JObject
            {
                ["name"] = item.Data.name,
                ["count"] = item.Count,
            });
        }

        jsonFile[INVENTORY_KEY] = inventoryJson;
    }

    public void AddToInventory(Item item, int amount)
    {
        if (inventory.Count >= 8) return;
        Debug.Log($"adding item {item.name} {amount}");

        // Find if the item already exists in the inventory
        var existingItem = inventory.FirstOrDefault(i => i.Data.name == item.name);
        if (existingItem != null)
        {
            // Update the count for the existing item
            existingItem.Count += amount;
            OnItemAdded?.Invoke(existingItem);
            Debug.Log($"incrementing item {item.name} count to {existingItem.Count}");
        }
        else
        {
            // Add a new item if it doesn't exist
            Debug.Log($"adding new item {item.name}");
            var targetItem = CreateInstance<ItemInstance>();
            targetItem.Initialize(item, amount);
            inventory.Add(targetItem);
            OnItemAdded?.Invoke(targetItem);
        }

        OnInventoryUpdated?.Invoke();
    }

    protected void RemoveFromInventory(Item item)
    {
        var existingItem = inventory.FirstOrDefault(i => i.Data.name == item.name);
        if (existingItem && existingItem.Count > 0)
        {
            existingItem.Count--;
            OnInventoryUpdated?.Invoke();
        }
        else
        {
            Debug.LogWarning("item does not exist in inventory");
        }
    }
}
