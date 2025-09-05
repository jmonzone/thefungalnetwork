using System;
using TMPro;
using UnityEngine;

[Serializable]
public class PartyRequirementUI : MonoBehaviour
{
    [SerializeField] private GameObject activeIcon;
    [SerializeField] private GameObject inactiveIcon;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;

    public void SetRequirement(PartyRequirementData requirement, bool value)
    {
        label.text = requirement.Label;

        activeIcon.SetActive(value);
        inactiveIcon.SetActive(!value);

        //label.fontStyle = value ? FontStyles.Normal : FontStyles.Strikethrough;
        label.color = value ? activeColor : inactiveColor;
    }
}
