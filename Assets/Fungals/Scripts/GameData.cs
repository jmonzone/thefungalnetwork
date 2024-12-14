using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemCollection : ScriptableObject
{
    [SerializeField] private List<Item> items;

    public List<Item> Items => items;

}
