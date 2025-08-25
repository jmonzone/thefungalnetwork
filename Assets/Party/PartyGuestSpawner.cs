using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyGuestSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnAnchor;
    [SerializeField] private UnitController unitPrefab;
    [SerializeField] private Unit guestUnit;

    private void Awake()
    {
        var partyManager = GetComponent<PartyHUDUI>();
        partyManager.OnGuestArrived += PartyReference_OnGuestArrived;
    }

    private void PartyReference_OnGuestArrived()
    {
        var guest = Instantiate(unitPrefab, spawnAnchor.transform.position, Quaternion.identity);
        guest.Initialize(guestUnit);
    }
}
