using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// implements a singleton GameManager to manage communication 
// with persistent global data across scenes
public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [Header("Services")]
    [SerializeField] private LocalData localData;
    [SerializeField] private Navigation uiNavigation;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private FadeCanvasGroup screenFade;
    [SerializeField] private MultiplayerManager multiplayer;

    [SerializeField] private DisplayName displayName;
    [SerializeField] private ItemInventory itemInventory;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private Volume volume;


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
            localData.Initialize();
            
            uiNavigation.Initialize();

            displayName.Initialize();

            // displayName needs to be initalized before multiplayer;
            multiplayer.Initialize();

            itemInventory.Initialize();
            fungalInventory.Initialize();

            //controller.Initialize(volume);

            DontDestroyOnLoad(instance);

            localData.OnReset += () =>
            {
                itemInventory.Initialize();
                fungalInventory.Initialize();
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

    private void Update()
    {
        multiplayer.DoUpdate();
    }
}
