using UnityEngine;

public class FeedPanel : MonoBehaviour
{
    [SerializeField] private InventoryList inventoryList;

    public PetInstance Pet { get; set; }

    private void Awake()
    {
        inventoryList.OnItemSelected += item =>
        {
            Pet.Hunger = 100;
        };
    }
}
