using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [SerializeField] private ItemInventory inventoryService;
    [SerializeField] private Item mushroomData;

    private ProximityAction proximityAction;

    private void Awake()
    {
        proximityAction = GetComponentInChildren<ProximityAction>();
        proximityAction.OnUse += () =>
        {
            inventoryService.AddToInventory(mushroomData, 1);
            proximityAction.SetInteractable(false);
            transform.localScale = Vector3.one * 0.1f;
        };
    }

    private void Update()
    {
        if (proximityAction.Interactable) return;

        if (transform.localScale.x < 1)
        {
            transform.localScale += 0.1f * Time.deltaTime * Vector3.one;
        }
        else
        {
            transform.localScale = Vector3.one;
            proximityAction.SetInteractable(true);
        }
    }
}
