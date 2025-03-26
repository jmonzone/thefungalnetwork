using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T GetRandomItem<T>(this List<T> items)
    {
        if (items.Count == 0) return default;

        var randomIndex = Random.Range(0, items.Count);
        return items[randomIndex];
    }
}