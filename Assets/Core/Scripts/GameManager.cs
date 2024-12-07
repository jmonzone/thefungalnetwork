using System.Collections.Generic;
using UnityEngine;

public static class ConfigKeys
{
    public const string CURRENT_PET_KEY = "currentPet";
    public const string LEVEL_KEY = "level";
    public const string EXPERIENCE_KEY = "experience";
    public const string HUNGER_KEY = "hunger";
    public const string NAME_KEY = "name";

    // stats
    public const string STATS_KEY = "stats";
    public const string BALANCE_KEY = "balance";
    public const string STAMINA_KEY = "stamina";
    public const string SPEED_KEY = "speed";
    public const string POWER_KEY = "power";
}

// handles persistent data across the game and provides an API to the save data
public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [Header("Services")]
    [SerializeField] private LocalData localData;
    [SerializeField] private Navigation navigation;
    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private DisplayName displayName;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;

            // order of initialization matters here, ability cast as of now
            // needs to be initialized before localdata.inventory does
            // or else there is unexpeted behaviour
            abilityCast.Reset();

            localData.Initialize();
            navigation.Initialize();

            displayName.Initialize();
            DontDestroyOnLoad(instance);
        }
    }

    private void Start()
    {
        //initialView.Open();
    }
}
