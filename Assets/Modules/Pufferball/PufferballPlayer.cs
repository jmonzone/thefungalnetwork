using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Events;

public class PufferballPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject fungal;

    public static UnityAction<Transform> OnLocalPlayerSpawned;
    public static UnityAction<Transform> OnRemotePlayerSpawned;

    protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
    {
        base.OnNetworkPreSpawn(ref networkManager);

        var partner = GameManager.Instance.GetPartner();

        if (partner)
        {
            SetRender(fungal);
        }
        else
        {
            SetRender(player);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            // This is the local player
            Debug.Log("Local player spawned: " + gameObject.name);
            OnLocalPlayerSpawned?.Invoke(transform);
        }
        else
        {
            // This is a remote player
            Debug.Log("Remote player spawned: " + gameObject.name);
            OnRemotePlayerSpawned?.Invoke(transform);
        }
    }

    private void SetRender(GameObject render)
    {
        render.SetActive(true);

        var animator = render.GetComponent<Animator>();

        var movementAnimations = GetComponent<MovementAnimations>();
        movementAnimations.SetAnimatior(animator);

        var networkAnimator = render.GetComponent<NetworkAnimator>();
        networkAnimator.Animator = animator;
    }
}
