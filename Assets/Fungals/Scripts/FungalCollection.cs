using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/New Fungal Collection")]
public class FungalCollection : ScriptableObject
{
    [SerializeField] private List<FungalData> data;

    public List<FungalData> Data => data;
}
