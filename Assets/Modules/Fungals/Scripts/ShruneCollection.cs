using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shrunes/New Shrune Collection")]
public class ShruneCollection : ScriptableObject
{
    [SerializeField] private List<ShruneItem> data;

    public List<ShruneItem> Data => data;
}
