using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Possessable : MonoBehaviour
{
    [SerializeField] private Color possessedColor;

    private MaterialFlasher materialFlasher;

    private void Awake()
    {
        materialFlasher = GetComponent<MaterialFlasher>();
    }

    public void OnPossess()
    {
        if (materialFlasher) materialFlasher.FlashColor(possessedColor);
    }
}
