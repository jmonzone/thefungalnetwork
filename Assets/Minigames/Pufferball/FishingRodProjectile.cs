using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class FishingRodProjectile : NetworkBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private GameObject render;
    [SerializeField] private float speed = 7.5f;
    [SerializeField] private float range = 5f;
    [SerializeField] private float detectionRadius = 1f;

    public Pufferfish Pufferfish { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        render.SetActive(false);
    }

    public void CastFishingRod(Vector3 direction)
    {
        if (Pufferfish && Pufferfish.IsCaught)
        {
            Pufferfish.Sling(direction);
        }
        else
        {
            Pufferfish = null;
            StopAllCoroutines();
            StartCoroutine(CastFishingRodRoutine(direction));
        }
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
                return true; // Collision detected
            }
        }
        return false;
    }

    private IEnumerator CastFishingRodRoutine(Vector3 direction)
    {
        Vector3 startPos = playerReference.Transform.position + direction.normalized;
        Vector3 targetPos = startPos + direction.normalized * range;

        // Hide the object initially
        RequestVisibilityServerRpc(false);

        // Tell the server to teleport it before clients see it
        TeleportServerRpc(startPos);

        yield return new WaitForSeconds(0.5f); // Ensure transform updates before showing

        // Now make it visible
        RequestVisibilityServerRpc(true);

        // Move forward
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            if (DetectPufferfishHit()) break;
            yield return null;
        }

        // Move back to the player
        while (Vector3.Distance(transform.position, playerReference.Transform.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerReference.Transform.position, speed * Time.deltaTime);
            yield return null;
        }

        if (Pufferfish) Pufferfish.PickUp();
        RequestVisibilityServerRpc(false);
    }

    // 🚀 **Server-side teleporting before clients see it**
    [ServerRpc(RequireOwnership = false)]
    private void TeleportServerRpc(Vector3 position)
    {
        transform.position = position;
        GetComponent<NetworkTransform>().Teleport(position, Quaternion.identity, Vector3.one);
    }

    // 🫥 **Toggle visibility across clients**
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
