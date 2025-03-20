using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/New Fungal Collection")]
public class FungalCollection : ScriptableObject
{
    [SerializeField] private List<FungalData> fungals;

    public List<FungalData> Fungals => fungals;
}
