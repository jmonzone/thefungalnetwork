using UnityEngine;

public class FeedPanel : MonoBehaviour
{
    [SerializeField] private InventoryList inventoryList;

    public FungalInstance Fungal { get; set; }

    private void Awake()
    {
        inventoryList.OnItemSelected += item =>
        {
            Fungal.Hunger = 100;
            item.Consume();
        };
    }
}
