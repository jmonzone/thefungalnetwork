using System.Collections;
using UnityEngine;

public static class ConfigKeys
{
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
    [SerializeField] private Tutorial tutorial;
    [SerializeField] private Navigation uiNavigation;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private FadeCanvasGroup screenFade;

    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private DisplayName displayName;
    [SerializeField] private ItemInventory itemInventory;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private Possession possession;


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
            
            uiNavigation.Initialize();
;
            tutorial.Initialize();
            displayName.Initialize();

            itemInventory.Initialize();
            fungalInventory.Initialize();
            possession.Initialize();

            DontDestroyOnLoad(instance);

            itemInventory.OnInventoryUpdated += ItemInventory_OnInventoryUpdated;
            ItemInventory_OnInventoryUpdated();

            localData.OnReset += () =>
            {
                abilityCast.Reset();
                itemInventory.Initialize();
                fungalInventory.Initialize();
                possession.Initialize();
            };

            sceneNavigation.OnSceneNavigationRequest += () =>
            {
                uiNavigation.Reset();
                StartCoroutine(sceneNavigation.NavigateToSceneRoutine(screenFade));
            };
        }
    }

    private IEnumerator Start()
    {
        screenFade.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        yield return screenFade.FadeOut();
        sceneNavigation.Initialize();
    }

    private void ItemInventory_OnInventoryUpdated()
    {
        if (!abilityCast.Shrune)
        {
            var shrune = itemInventory.Items.Find(item => item.Data is ShruneItem);
            if (shrune) abilityCast.SetShrune(shrune.Data as ShruneItem);
        }
    }
}
