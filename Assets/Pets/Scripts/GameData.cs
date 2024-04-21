using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    [SerializeField] private List<Pet> pets;

    public List<Pet> Pets => pets;
}
