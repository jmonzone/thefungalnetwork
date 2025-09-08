using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FungalUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnitListReference unitListReference;
    [SerializeField] private Image fungalImage;
    [SerializeField] private TextMeshProUGUI fungalName;
    [SerializeField] private Slider relationshipSlider;
    [SerializeField] private Image headImage;
    [SerializeField] private TextMeshProUGUI dateText;

    private void OnEnable()
    {
        unitListReference.OnFungalSelected += UnitListReference_OnFungalSelected;
    }

    private void OnDisable()
    {
        unitListReference.OnFungalSelected -= UnitListReference_OnFungalSelected;
    }

    private void UnitListReference_OnFungalSelected(Unit unit)
    {
        SetFungal(unit);
    }

    public void SetFungal(Unit unit)
    {
        fungalImage.sprite = unit.Sprite;
        fungalName.name = unit.Name;
        dateText.text = DateTime.Now.ToLongDateString();
    }
}
