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
            if (anchor) return anchor.position;
            else return Vector3.zero;
        }
    }

    public Collider Bounds
    {
        get => bounds;
        set => bounds = value;
    }
}
