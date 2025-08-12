using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask collisionMask;

    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    [SerializeField] private InventoryReference inventory;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference buildView;
    [SerializeField] private Button buildButton;
    [SerializeField] private Button closeButton;

    private List<ItemUI> itemViewList = new List<ItemUI>();

    private BuildController itemContoller;

    private void Awake()
    {
        GetComponentsInChildren(true, itemViewList);

        foreach(var itemView in itemViewList)
        {
            itemView.OnClick += () => ItemView_OnClick(itemView.Item);
        }

        var viewController = GetComponent<ViewController>();
        viewController.OnFadeInStart += ViewController_OnFadeInStart;

        buildButton.onClick.AddListener(() =>
        {
            if (itemContoller)
            {
                itemContoller.CompleteBuild();
                itemContoller = null;
                navigation.GoBack();
            }
        });

        closeButton.onClick.AddListener(() =>
        {
            if (itemContoller)
            {
                Destroy(itemContoller.gameObject);
                itemContoller = null;
                navigation.GoBack();
            }
        });
    }

    private void ItemView_OnClick(Item item)
    {
        itemContoller = Instantiate(item.ItemPrefab).GetComponent<BuildController>();
        itemContoller.StartBuild(groundMask, collisionMask, validMaterial, invalidMaterial);

        navigation.Navigate(buildView);

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
