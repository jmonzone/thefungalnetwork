using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class InventoryReference : ScriptableObject
{
    [SerializeField] private int sporeCount = 0;
    [SerializeField] private int initialSporeCount = 124;

    public int SporeCount => sporeCount;

    public event UnityAction<int> OnSporeCountChanged;

    public void Initialize()
    {
        sporeCount = initialSporeCount;
    }

    public void IncreaseSporeCount(int value = 1)
    {
        sporeCount += value;
        OnSporeCountChanged?.Invoke(sporeCount);
    }
}
