using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;


[CreateAssetMenu]
public class PartyLogReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private LocalData localData;
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private List<PartyData> allParties;

    [Header("Runtime")]
    [SerializeField] private List<PartyData> completedParties;

    public List<PartyData> CompletedParties => completedParties;

    private const string PARTY_KEY = "parties";

    public void Initialize()
    {
        try
        {
            completedParties = new List<PartyData>();

            if (localData.JsonFile.ContainsKey(PARTY_KEY))
            {
                foreach (var json in localData.JsonFile[PARTY_KEY] as JArray)
                {
                    if (json is JObject partyJson)
                    {
                        var partyData = allParties.Find(quest => quest.Name == partyJson["name"].ToString());
                        completedParties.Add(partyData);
                    };
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        partyReference.OnPartyComplete += PartyReference_OnPartyComplete;
    }

    private void PartyReference_OnPartyComplete()
    {
        completedParties.Add(partyReference.CurrentParty);
        SaveData();
    }

    private void SaveData()
    {
        var partyJson = new JArray();

        foreach (var party in completedParties)
        {
            partyJson.Add(new JObject
            {
                ["name"] = party.Name
            });
        }

        localData.SaveData(PARTY_KEY, partyJson);
    }
}
