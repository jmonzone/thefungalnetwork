using System.Collections.Generic;
using UnityEngine;

public class BuildUI : MonoBehaviour
{
    [SerializeField] private InventoryReference inventory;

    private List<ItemUI> itemViewList = new List<ItemUI>();

    private void Awake()
    {
        GetComponentsInChildren(true, itemViewList);

        foreach(var itemView in itemViewList)
        {
            itemView.OnClick += () => ItemView_OnClick(itemView.Item);
        }

        var viewController = GetComponent<ViewController>();
        viewController.OnFadeInStart += ViewController_OnFadeInStart;
    }

    private void ItemView_OnClick(Item item)
    {
        Instantiate(item.ItemPrefab);
    }

    private void ViewController_OnFadeInStart()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        var i = 0;
        foreach (var itemView in itemViewList)
        {
            if (i < inventory.Items.Count)
            {
                var item = inventory.Items[i];
                itemView.SetItem(item);
            }
            else
            {
                itemView.SetItem(null);
            }

            //itemView.gameObject.SetActive(true);

            i++;
        }
    }
}
