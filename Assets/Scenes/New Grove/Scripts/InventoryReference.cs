using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class InventoryReference : ScriptableObject
{
    [SerializeField] private int sporeCount = 0;
    [SerializeField] private int initialSporeCount = 124;
    [SerializeField] private bool hasDJTable = false;

    public int SporeCount => sporeCount;
    public bool HasDJTable => hasDJTable;

    public event UnityAction<int> OnSporeCountChanged;
    public event UnityAction OnItemSummoned;

    public void Initialize()
    {
        sporeCount = initialSporeCount;
        hasDJTable = false;
    }

    public void IncreaseSporeCount(int value = 1)
    {
        sporeCount += value;
        OnSporeCountChanged?.Invoke(sporeCount);
    }

    public void DecreaseSporeCount(int value = 1)
    {
        sporeCount -= value;
        OnSporeCountChanged?.Invoke(sporeCount);
    }

    public void SummonItem(int price)
    {
        DecreaseSporeCount(price);
        hasDJTable = true;
        OnItemSummoned?.Invoke();
    }
}
