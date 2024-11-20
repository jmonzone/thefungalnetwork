using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [SerializeField] private Item mushroomData;

    private void Awake()
    {
        var proximityAction = GetComponentInChildren<ProximityAction>();
        proximityAction.OnUse += () => GameManager.Instance.AddToInventory(mushroomData, 1);
    }
}
