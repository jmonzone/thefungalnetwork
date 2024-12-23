using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class BogFrog : NetworkBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private List<MiniFrog> miniFrogs;

    public float shrinkSpeed = 1f; // Speed at which the parent shrinks
    public float miniSpawnHeight = 2f; // Height offset for mini versions

    public IEnumerator HandleAnimation()
    {
        yield return new WaitForSeconds(2f);
        var animator = GetComponentInChildren<Animator>();
        animator.speed = 0.25f;
        animator.Play("Death");
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator ShrinkDown()
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
    }

    private void SpawnMiniObjects()
    {
        for (int i = 0; i < miniFrogs.Count; i++)
        {
            miniFrogs[i].AssignSpore(arena.Spores[i], arena.SporePositions[i]);
        }
    }
}
