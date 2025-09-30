using UnityEngine;

[CreateAssetMenu]
public class DanceMove : ScriptableObject
{
    [SerializeField] private string label;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string animationName;
    [SerializeField] private float xp = 5;

    public string Label => label;
    public Sprite Sprite => sprite;
    public string AnimationName => animationName;
    public float Xp => xp;
}
