using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkDisplayName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayNameText;

    private void OnEnable()
    {
        var fungal = GetComponentInParent<NetworkFungal>();
        displayNameText.text = fungal.playerName.Value.ToString();

        fungal.playerName.OnValueChanged += (old, value) =>
        {
            displayNameText.text = value.ToString();
        };
    }
}
