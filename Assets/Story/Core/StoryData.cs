using UnityEngine;

[CreateAssetMenu]
public class StoryData : ScriptableObject
{
    [SerializeField] private string id;
    public string Id => id;
}
