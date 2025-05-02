using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/Power Ups/New Power Up Collection")]
public class PowerUpCollection : ScriptableObject
{
    [SerializeField] private List<Ability> powerUps;

    public List<Ability> PowerUps => powerUps;
}
