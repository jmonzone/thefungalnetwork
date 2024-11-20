using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroveUI : MonoBehaviour
{
    [SerializeField] private bool debug;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject inputUI;
    [SerializeField] private InventoryUI inventoryUI;

    [SerializeField] private Button inventoryButton;
    [SerializeField] private Image inventoryPreview;

    private void Awake()
    {
        inventoryButton.onClick.AddListener(() =>
        {
            ToggleUI(false);
        });

        inventoryUI.OnCloseButtonClicked += () => ToggleUI(true);

        gameManager.OnItemAdded += item =>
        {
            inventoryPreview.enabled = true;
            inventoryPreview.sprite = item.Data.Sprite;
        };

        if (!debug || !Application.isEditor)
        {
            ToggleUI(true);
        }
    }

    private void ToggleUI(bool input)
    {
        inputUI.SetActive(input);
        inventoryUI.gameObject.SetActive(!input);
    }
}
