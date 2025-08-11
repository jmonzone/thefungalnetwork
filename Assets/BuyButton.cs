using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour
{
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject inactiveLabel;
    [SerializeField] private GameObject activeLabel;
    [SerializeField] private Button button;

    private int price;

    public event UnityAction OnBuy;

    private void Awake()
    {
        button.onClick.AddListener(() => OnBuy?.Invoke());
    }

    private void OnEnable()
    {
        inventory.OnSporeCountChanged += Inventory_OnSporeCountChanged;
    }

    private void OnDisable()
    {
        inventory.OnSporeCountChanged -= Inventory_OnSporeCountChanged;
    }

    private void Inventory_OnSporeCountChanged(int arg0)
    {
        UpdateView();
    }

    public void SetPrice(int price)
    {
        this.price = price;
        priceText.text = price.ToString();
        UpdateView();
    }

    private void UpdateView()
    {
        var canBuy = inventory.SporeCount >= price;

        button.interactable = canBuy;
        activeLabel.SetActive(canBuy);
        inactiveLabel.SetActive(!canBuy);
    }
}
