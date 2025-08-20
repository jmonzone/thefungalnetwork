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

        var viewController = GetComponent<ViewController>();
        viewController.OnFadeInStart += ViewController_OnFadeInStart;
    }

    private void ViewController_OnFadeInStart()
    {
        foreach (var page in partyPages)
        {
            page.SetParty(initialParty);
        }
    }
}
