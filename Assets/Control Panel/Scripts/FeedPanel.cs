using UnityEngine;

public class FeedPanel : MonoBehaviour
{
    [SerializeField] private InventoryList inventoryList;

    public FungalInstance Fungal { get; set; }

    private void Awake()
    {
        inventoryList.OnItemSelected += item =>
        {
            if (item.Data is FishData fish)
            {
                Fungal.Hunger = 100;
                foreach(var stat in fish.StatGrowth)
                {
                    switch(stat.Type)
                    {
                        case StatType.BALANCE:
                            Fungal.Balance += stat.Growth;
                            break;
                        case StatType.SPEED:
                            Fungal.Speed += stat.Growth;
                            break;
                        case StatType.STAMINA:
                            Fungal.Stamina += stat.Growth;
                            break;
                        case StatType.POWER:
                            Fungal.Power += stat.Growth;
                            break;
                    }
                }

                item.Consume();
            }
        };
    }
}
