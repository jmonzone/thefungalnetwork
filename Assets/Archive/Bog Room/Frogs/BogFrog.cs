using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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
}
