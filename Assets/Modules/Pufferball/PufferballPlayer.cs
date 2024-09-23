using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Events;

public class PufferballPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private FungalCollection fungalCollection;

    public static UnityAction<Transform> OnLocalPlayerSpawned;
    public static UnityAction<Transform> OnRemotePlayerSpawned;

    protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
    {
        base.OnNetworkPreSpawn(ref networkManager);

        if (!TrySpawnPartner()) SetRender(player);

        transform.position = new Vector3(0, 2, -4);
    }

    private bool TrySpawnPartner()
    {
        var partner = GameManager.Instance.GetPartner();

        if (partner)
        {
            var targetFungal = fungalCollection.Data.Find(fungal => fungal.Id == partner?.Data.Id);

            if (targetFungal)
            {
                Debug.Log("target found");
                var render = Instantiate(targetFungal.Prefab, transform);
                SetRender(render);
                return true;
            }

            Debug.Log("no target found");

        }

        return false;
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

        var ownerNetworkAnimator = render.GetComponent<OwnerNetworkAnimator>();
        ownerNetworkAnimator.Animator = animator;
    }
}
