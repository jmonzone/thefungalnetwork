using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;
}
