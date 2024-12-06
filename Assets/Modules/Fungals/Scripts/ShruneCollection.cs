using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shrunes/New Shrune Collection")]
public class ShruneCollection : ScriptableObject
{
    [SerializeField] private List<ShruneItem> data;

    public List<ShruneItem> Data => data;

    public bool TryGetShruneById(string id, out ShruneItem shrune)
    {
        shrune = Data.Find(shrune => shrune.name == id);
        return shrune;
    }


}
