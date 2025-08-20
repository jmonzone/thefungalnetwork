using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyUI : MonoBehaviour
{
    [SerializeField] private PartyData initialParty;

    private List<PartyPageUI> partyPages = new List<PartyPageUI>();

    private void Awake()
    {
        GetComponentsInChildren(true, partyPages);

        foreach(var page in partyPages)
        {
            page.SetParty(initialParty);
        }
    }
}
