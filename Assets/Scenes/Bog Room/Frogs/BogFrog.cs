using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BogFrog : NetworkBehaviour
{
    public GameObject miniPrefab;  // Reference to the prefab of the mini version
    public float shrinkSpeed = 1f; // Speed at which the parent shrinks
    public float miniSpawnHeight = 2f; // Height offset for mini versions

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            var animator = GetComponentInChildren<Animator>();
            animator.Play("Jump");

            // Start shrinking the parent object
            StartCoroutine(ShrinkParent());
        }
    }

    private IEnumerator ShrinkParent()
    {
        // Shrink the parent object
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;
        float elapsedTime = 0f;

        while (elapsedTime < shrinkSpeed)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / shrinkSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the parent object is fully shrunk
        transform.localScale = targetScale;

        // Spawn the mini versions
        SpawnMiniObjects();
    }

    private void SpawnMiniObjects()
    {
        for (int i = 0; i < 5; i++)
        {
            // Instantiate mini versions at different positions above the parent
            Vector3 spawnPosition = transform.position + new Vector3(i * 1f, miniSpawnHeight, 0f);
            Instantiate(miniPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
