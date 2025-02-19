using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class FishingRodProjectile : NetworkBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private PufferballReference pufferballReference;

    [SerializeField] private GameObject render;
    [SerializeField] private float speed = 7.5f;
    [SerializeField] private float range = 5f;
    [SerializeField] private float detectionRadius = 1f;

    public Pufferfish Pufferfish { get; private set; }
    public event UnityAction OnPufferfishReleased;

    private void Update()
    {
        if (IsOwner && !render.activeSelf)
        {
            transform.position = playerReference.Movement.transform.position;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        render.SetActive(false);

        var networkPufferfish = FindObjectOfType<Pufferfish>(); // Ensure it's the correct one
        if (networkPufferfish != null)
        {
            networkPufferfish.OnMaxTemperReached += NetworkPufferfish_OnMaxTemperReached;
        }
    }

    private void NetworkPufferfish_OnMaxTemperReached()
    {
        Pufferfish = null;
        OnPufferfishReleased?.Invoke();
    }

    public void Sling(Vector3 direction)
    {
        if (Pufferfish)
        {
            Pufferfish.Sling(direction);
            Pufferfish = null;
        }
    }

    public void Cast(Vector3 direction, UnityAction<bool> onComplete)
    {
        StopAllCoroutines();
        StartCoroutine(CastFishingRodRoutine(direction, onComplete));
    }

    private IEnumerator CastFishingRodRoutine(Vector3 direction, UnityAction<bool> onComplete)
    {
        Vector3 startPos = playerReference.Movement.transform.position + direction.normalized;
        Vector3 targetPos = startPos + direction.normalized * range;

        // Now make it visible
        RequestVisibilityServerRpc(true);

        transform.position = startPos;

        // Move forward
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            if (DetectPufferfishHit()) break;
            yield return null;
        }

        // Move back to the player
        while (Vector3.Distance(transform.position, playerReference.Movement.transform.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerReference.Movement.transform.position, speed * Time.deltaTime);
            yield return null;
        }

        if (Pufferfish) Pufferfish.PickUp();
        onComplete?.Invoke(Pufferfish);
        RequestVisibilityServerRpc(false);

    }

    private bool DetectPufferfishHit()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f); // Small detection radius

        foreach (Collider hit in hits)
        {
            Pufferfish = hit.GetComponentInParent<Pufferfish>();
            if (Pufferfish != null)
            {
                Pufferfish.Catch(transform);
                return true;
            }
        }
        return false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestVisibilityServerRpc(bool isVisible)
    {
        RequestVisibilityClientRpc(isVisible);
    }

    [ClientRpc]
    private void RequestVisibilityClientRpc(bool isVisible)
    {
        render.SetActive(isVisible);
    }
}
