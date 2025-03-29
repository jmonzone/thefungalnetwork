using TMPro;
using UnityEngine;

public class NetworkDisplayName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayNameText;

    private NetworkFungal fungal;

    private void Awake()
    {
        fungal = GetComponentInParent<NetworkFungal>();
        fungal.OnPlayerUpdated += UpdatePlayerName;

        UpdatePlayerName();
    }

    private void UpdatePlayerName()
    {
        displayNameText.text = fungal.PlayerName.ToString();
    }
}
