using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Events;

public class PufferballPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private PufferballController pufferballPrefab;

    [SerializeField] private PufferballController pufferball;
    private NetworkTransform networkTransform;

    public static UnityAction<Transform> OnLocalPlayerSpawned;
    public static UnityAction<Transform> OnRemotePlayerSpawned;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            networkTransform = GetComponent<NetworkTransform>();

            // This is the local player
            Debug.Log("Local player spawned: " + gameObject.name);
            if (!TrySpawnPartner()) SetRender(player);

            if (IsServer)
            {
                var spawnPosition = new Vector3(0 , 2, 0);
                // Instantiate the object only on the server
                pufferball = Instantiate(pufferballPrefab, spawnPosition, Quaternion.identity);
                // Spawn the object across the network
                pufferball.GetComponent<NetworkObject>().Spawn();

                Quaternion forwardRotation = Quaternion.LookRotation(Vector3.forward);

                networkTransform.Teleport(new Vector3(0, 2, -4), forwardRotation, Vector3.one);
            }
            else
            {
                Quaternion backRotation = Quaternion.LookRotation(Vector3.back);

                networkTransform.Teleport(new Vector3(0, 2, 4), backRotation, Vector3.one);
            }

            OnLocalPlayerSpawned?.Invoke(transform);
        }
        else
        {
            // This is a remote player
            Debug.Log("Remote player spawned: " + gameObject.name);
            SetRender(player);
            OnRemotePlayerSpawned?.Invoke(transform);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            CatchBall();
        }

    }

    private void CatchBall()
    {
        pufferball.transform.position = transform.position + Vector3.up * 2;

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

    private void SetRender(GameObject render)
    {
        Debug.Log("setting render");
        render.SetActive(true);

        var animator = render.GetComponent<Animator>();
        Debug.Log("getting animator");

        var movementAnimations = GetComponent<MovementAnimations>();
        movementAnimations.SetAnimatior(animator);
        Debug.Log("getting movementAnimations");

        var ownerNetworkAnimator = render.GetComponent<OwnerNetworkAnimator>();
        ownerNetworkAnimator.Animator = animator;

        Debug.Log("getting ownerNetworkAnimator");
    }
}
