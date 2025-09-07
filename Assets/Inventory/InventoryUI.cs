using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private InventoryItemUI itemViewPrefab;

    private List<InventoryItemUI> itemViewList = new List<InventoryItemUI>();

    private void Awake()
    {
        GetComponentsInChildren(true, itemViewList);
    }

    private void Start()
    {
        UpdateView();
    }

    private void OnEnable()
    {
        inventory.OnInventoryOpened += UpdateView;
    }

    private void OnDisable()
    {
        inventory.OnInventoryOpened -= UpdateView;
    }

    private void UpdateView()
    {
        int itemCount = inventory.Items.Count;

        var sortedItems = inventory.Items.OrderBy(item => item.Price).ToList();

        // Ensure we have enough views
        while (itemViewList.Count < itemCount)
        {
            // Instantiate new ItemView if needed
            var newView = Instantiate(itemViewPrefab, transform);
            itemViewList.Add(newView);
        }

        // Update active views
        for (int i = 0; i < itemViewList.Count; i++)
        {
            if (i < itemCount)
            {
                itemViewList[i].SetItem(sortedItems[i]);
                itemViewList[i].gameObject.SetActive(true);
            }
            else
            {
                itemViewList[i].SetItem(null);
                itemViewList[i].gameObject.SetActive(false);
            }
        }
    }

}
