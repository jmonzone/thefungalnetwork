using UnityEngine;

[CreateAssetMenu]
public class DanceMove : ScriptableObject
{
    [SerializeField] private string label;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string animationName;

    public string Label => label;
    public Sprite Sprite => sprite;
    public string AnimationName => animationName;
}
