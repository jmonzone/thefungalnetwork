using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyInviteUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Toggle toggle;

    public void SetUnit(UnitInstance unit)
    {
        image.sprite = unit.Data.Sprite;
        image.enabled = true;
        text.text = unit.Data.Name;
        toggle.isOn = true;
        toggle.interactable = true;
    }

    public void Clear()
    {
        toggle.interactable = false;
        toggle.isOn = false;
        image.enabled = false;
        text.text = "";
    }
}
