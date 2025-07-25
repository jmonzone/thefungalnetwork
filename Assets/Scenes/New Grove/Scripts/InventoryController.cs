using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public interface ICollectable
{
    public event UnityAction OnCollect;
}

public class InventoryController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mushroomCountText;

    private int mushroomCount;

    private void Awake()
    {
        var mushrooms = FindObjectsOfType<InteractableMushroom>(true);
        foreach(var mushroom in mushrooms)
        {
            mushroom.OnCollect += () =>
            {
                mushroomCount++;
                mushroomCountText.text = mushroomCount.ToString();
            };
        }
    }
}
