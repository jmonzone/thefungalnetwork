using System;
using TMPro;
using UnityEngine;

[Obsolete]
public class DisplayNameInput : MonoBehaviour
{
    [SerializeField] private DisplayName displayName;
    [SerializeField] private TMP_InputField inputField;

    private void Start()
    {
        //inputField.onValueChanged.AddListener(value => displayName.SetValue(value));
    }

    private void OnEnable()
    {
        inputField.text = displayName.Value;
    }
}
