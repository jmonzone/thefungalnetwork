using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeArena : MonoBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private List<Transform> spawnPositions;

    private void Awake()
    {
        arena.Initialize(spawnPositions);
    }
}
