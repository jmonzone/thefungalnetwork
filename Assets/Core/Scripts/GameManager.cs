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

public static class SceneParameters
{
    public static int FungalIndex = 0;
}

// handles persistent data across the game and provides an API to the save data
public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField] private LocalDataService localDataService;
    [SerializeField] private Navigation uiStateService;

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
            localDataService.Initialize();
            uiStateService.Initialize();
            DontDestroyOnLoad(instance);
        }
    }
}
