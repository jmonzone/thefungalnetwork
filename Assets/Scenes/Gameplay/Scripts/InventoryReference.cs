using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class InventoryReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private LocalData localData;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference inventoryView;

    [Header("Settings")]
    [SerializeField] private int initialSporeCount = 124;
    [SerializeField] private List<Item> initialItems;

    [Header("Runtime")]
    [SerializeField] private int sporeCount = 0;
    [SerializeField] private List<Item> items;

    public int SporeCount => sporeCount;
    public List<Item> Items => items;

    public event UnityAction<SporeController> OnSporeCollected;
    public event UnityAction<int> OnSporeCountChanged;
    public event UnityAction OnItemSummoned;
    public event UnityAction OnInventoryOpened;
    public event UnityAction<Item> OnItemSelected;

    private const string SPORE_KEY = "spore";

    public void Initialize()
    {
        items = new List<Item>(initialItems);

        if (localData.JsonFile.ContainsKey(SPORE_KEY))
        {
            var sporeJson = localData.JsonFile[SPORE_KEY];
            sporeCount = (int)sporeJson;
        }
        else
        {
            sporeCount = initialSporeCount;
        }
    }

    public void CollectSpore(SporeController sporeController)
    {
        IncreaseSporeCount(1);
        OnSporeCollected?.Invoke(sporeController);
    }

    public void IncreaseSporeCount(int value = 1)
    {
        sporeCount += value;
        OnSporeCountChanged?.Invoke(sporeCount);
        localData.SaveData(SPORE_KEY, sporeCount);
    }

    public void DecreaseSporeCount(int value = 1)
    {
        sporeCount -= value;
        OnSporeCountChanged?.Invoke(sporeCount);
        localData.SaveData(SPORE_KEY, sporeCount);
    }

    public void SummonItem(Item item)
    {
        items.Add(item);
        DecreaseSporeCount(item.Price);
        OnItemSummoned?.Invoke();
    }

    public void OpenInventory()
    {
        navigation.Navigate(inventoryView);
        OnInventoryOpened?.Invoke();
    }

    public void SelectItem(Item item)
    {
        OnItemSelected?.Invoke(item);
    }
}
