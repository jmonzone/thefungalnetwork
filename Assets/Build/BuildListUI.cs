using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuildListUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BuildReference build;
    [SerializeField] private InventoryReference inventory;

    [Header("UI References")]
    [SerializeField] private BuildItemUI itemViewPrefab;
    [SerializeField] private Button closeButton;

    private List<BuildItemUI> itemViewList = new List<BuildItemUI>();

    private void Awake()
    {
        GetComponentsInChildren(true, itemViewList);

        foreach(var itemView in itemViewList)
        {
            itemView.OnClick += () => build.StartBuildPlacement(itemView.Item);
        }

        closeButton.onClick.AddListener(build.EndBuild);
    }

    private void Start()
    {
        UpdateView();
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
