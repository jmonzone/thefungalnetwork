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

    private void UnitListReference_OnFungalSelected(UnitInstance unit)
    {
        SetFungal(unit);
    }

    public void SetFungal(UnitInstance unit)
    {
        fungalImage.sprite = unit.Data.Sprite;
        fungalName.text = unit.Data.Name;
        relationshipSlider.minValue = 0;
        relationshipSlider.maxValue = 8;
        relationshipSlider.value = unit.Relationship;
        dateText.text = DateTime.Now.ToLongDateString();
    }
}
