using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyGuestSpawner : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;

    [SerializeField] private Transform spawnAnchor;
    [SerializeField] private UnitController unitPrefab;
    [SerializeField] private Unit guestUnit;

    private void OnEnable()
    {
        partyReference.OnGuestArrived += PartyReference_OnGuestArrived;
    }

    private void OnDisable()
    {
        partyReference.OnGuestArrived -= PartyReference_OnGuestArrived;
    }

    private void PartyReference_OnGuestArrived()
    {
        var guest = Instantiate(unitPrefab, spawnAnchor.transform.position, Quaternion.identity);
        guest.Initialize(guestUnit);
    }
}
