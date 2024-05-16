using System.Collections.Generic;
using UnityEngine;


public class Item : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;

    public string Name => name;
    public Sprite Sprite => sprite;
}

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    [SerializeField] private List<Pet> pets;
    [SerializeField] private List<Item> items;

    public List<Pet> Pets => pets;
    public List<Item> Items => items;

}
