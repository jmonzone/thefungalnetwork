using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TelegraphTrajectory : NetworkBehaviour
{
    [SerializeField] private GameObject radiusIndicator;

    private void Awake()
    {
        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowStart += targetPosition => OnThrowStartServerRpc(targetPosition, throwFish.Radius);
        throwFish.OnThrowComplete += OnThrowCompleteServerRpc;
    }

    [ServerRpc]
    public void OnThrowStartServerRpc(Vector3 targetPosition, float radius)
    {
        OnThrowStartClientRpc(targetPosition, radius);
    }

    [ClientRpc]
    private void OnThrowStartClientRpc(Vector3 targetPosition, float radius)
    {
        radiusIndicator.transform.parent = null;
        radiusIndicator.transform.position = targetPosition + Vector3.up * 0.1f;
        radiusIndicator.SetActive(true);
        radiusIndicator.transform.localScale = 2f * radius * Vector3.one;
    }

    [ServerRpc]
    public void OnThrowCompleteServerRpc()
    {
        OnThrowCompleteClientRpc();
    }

    [ClientRpc]
    private void OnThrowCompleteClientRpc()
    {
        radiusIndicator.SetActive(false);
    }
}
