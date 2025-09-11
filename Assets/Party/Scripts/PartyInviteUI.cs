using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyInviteUI : MonoBehaviour
{
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image invitedByImage;

    public void SetUnit(UnitInstance unit, bool autoInvited)
    {
        unitImage.sprite = unit.Data.Sprite;
        unitImage.enabled = true;
        text.text = unit.Data.Name;
        toggle.isOn = true;
        toggle.interactable = !autoInvited;
        invitedByImage.enabled = autoInvited;
    }

    public void Clear()
    {
        toggle.interactable = false;
        toggle.isOn = false;
        unitImage.enabled = false;
        text.text = "";
        invitedByImage.enabled = false;
    }
}
