using System;
using UnityEngine;

[Serializable]
public class PositionAnchor
{
    [SerializeField] private Transform anchor;
    [SerializeField] private Collider bounds;

    public bool IsInitialized => anchor || bounds;

    public Vector3 Position
    {
        get
        {

            if (bounds) return bounds.GetRandomXZPosition();
            return anchor.position;
        }
    }

    public Collider Bounds
    {
        get => bounds;
        set => bounds = value;
    }
}
