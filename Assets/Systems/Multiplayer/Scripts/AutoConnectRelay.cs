using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoConnectRelay : MonoBehaviour
{
    private void Start()
    {
        if (MultiplayerManager.Instance.JoinedLobby != null)
        {
            MultiplayerManager.Instance.CreateRelay();
        }
    }
}
