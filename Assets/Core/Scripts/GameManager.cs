using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// implements a singleton GameManager to manage communication 
// with persistent global data across scenes
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Services")]
    [SerializeField] private LocalData localData;
    [SerializeField] private Navigation uiNavigation;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private FadeCanvasGroup screenFade;

    [SerializeField] private DisplayName displayName;
    [SerializeField] private BuildReference build;
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private UnitListReference unitList;
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private SporeReference sporeReference;
    [SerializeField] private Volume volume;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;


            // order of initialization matters here, ability cast as of now
            // needs to be initialized before localdata.inventory does
            // or else there is unexpeted behaviour
            localData.Initialize();

            inventory.Initialize();
            build.Initialize();
            unitList.Initialize();
            partyReference.Initialize();
            sporeReference.Initialize();

            uiNavigation.Initialize();

            displayName.Initialize();


            DontDestroyOnLoad(Instance);

            localData.OnReset += () =>
            {
                inventory.Initialize();
                build.Initialize();
                unitList.Initialize();
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
        //multiplayer.DoUpdate();
    }
}
