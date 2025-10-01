using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Skill : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private Sprite sprite;

    public string Id => id;
    public Sprite Sprite => sprite;

}
