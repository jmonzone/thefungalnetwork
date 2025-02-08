using UnityEngine;

[CreateAssetMenu]
public class Controller : ScriptableObject
{
    public Transform Transform { get; private set; }

    public void SetTransform(Transform transform)
    {
        Transform = transform;
    }
}
