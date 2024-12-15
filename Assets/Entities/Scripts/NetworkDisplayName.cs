using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkDisplayName : NetworkBehaviour
{
    [SerializeField] private DisplayName displayName;
    [SerializeField] private TextMeshProUGUI displayNameText;

    private NetworkVariable<FixedString128Bytes> nameNetwork = new NetworkVariable<FixedString128Bytes>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Debug.Log($"Spawned IsOwner:{IsOwner}");
        if (IsOwner)
        {
            //displayNameText.gameObject.SetActive(false);
            displayNameText.text = displayName.Value;
            UpdateDisplayNameServerRpc(new FixedString64Bytes(displayName.Value));
        }
        else
        {
            displayNameText.text = nameNetwork.Value.ToString();
        }
    }

    [ServerRpc]
    private void UpdateDisplayNameServerRpc(FixedString64Bytes name)
    {
        nameNetwork.Value = name;
        UpdateDisplayNameClientRpc(name);
    }

    [ClientRpc]
    private void UpdateDisplayNameClientRpc(FixedString64Bytes name)
    {
        displayNameText.text = name.ToString();
    }

}
