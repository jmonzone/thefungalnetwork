using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    [SerializeField] private List<FungalData> fungals;
    [SerializeField] private List<Item> items;

    public List<FungalData> Fungals => fungals;
    public List<Item> Items => items;

}
