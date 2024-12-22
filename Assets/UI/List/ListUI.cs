using System.Collections.Generic;
using UnityEngine;

namespace GURU
{
    /// <summary>
    /// Manages a list of UI items, allowing for dynamic creation and updating of list item data.
    /// </summary>
    public class ListUI : MonoBehaviour
    {
        /// <summary>
        /// The anchor point for positioning list items.
        /// </summary>
        [SerializeField, Tooltip("The Transform where list items will be anchored.")]
        private Transform itemAnchor;

        /// <summary>
        /// The prefab used to create list items.
        /// </summary>
        [SerializeField, Tooltip("The prefab used to instantiate list items.")]
        private ListItemUI itemPrefab;

        private List<ListItemUI> items = new List<ListItemUI>();

        /// <summary>
        /// Initializes the list UI by retrieving existing list items.
        /// </summary>
        private void Awake()
        {
            itemAnchor.GetComponentsInChildren(includeInactive: true, items);

            foreach (var item in items)
            {
                item.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Sets the items displayed in the list based on the provided data.
        /// </summary>
        /// <param name="data">A list of data representing the items to display.</param>
        public void SetItems(List<ListItemData> data)
        {
            int existingCount = items.Count;
            int requiredCount = data.Count;

            if (existingCount < requiredCount)
            {
                for (int i = existingCount; i < requiredCount; i++)
                {
                    var item = Instantiate(itemPrefab, itemAnchor);
                    items.Add(item);
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (i < requiredCount)
                {
                    items[i].SetData(data[i]);
                }
                else
                {
                    items[i].gameObject.SetActive(false);
                }
            }
        }
    }
}