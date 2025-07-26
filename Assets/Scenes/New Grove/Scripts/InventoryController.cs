using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public interface ICollectable
{
    public Transform Transform { get; }
    public event UnityAction OnCollect;
}

public class InventoryController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mushroomCountText;

    public int MushroomCount { get; private set; }
    public event UnityAction<ICollectable> OnCollect;
    public event UnityAction<int> OnMushroomCountChanged;

    private void Awake()
    {
        var mushrooms = FindObjectsOfType<InteractableMushroom>(true);
        foreach(var mushroom in mushrooms)
        {
            mushroom.OnCollect += () =>
            {
                MushroomCount++;
                mushroomCountText.text = MushroomCount.ToString();
                OnMushroomCountChanged?.Invoke(MushroomCount);
                OnCollect?.Invoke(mushroom);
            };
        }
    }

    public void ConsumeMushroom(int count)
    {
        MushroomCount -= count;
        mushroomCountText.text = MushroomCount.ToString();
        OnMushroomCountChanged?.Invoke(MushroomCount);
    }
}
