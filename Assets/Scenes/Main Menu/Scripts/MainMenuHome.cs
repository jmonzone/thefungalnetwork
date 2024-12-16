using UnityEngine;
using UnityEngine.UI;

public class MainMenuHome : MonoBehaviour
{
    [SerializeField] private Button partyButton;
    [SerializeField] private Button adventureButton;
    [SerializeField] private Button resetButton;

    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewController viewController;
    [SerializeField] private ViewReference matchmakingView;
    [SerializeField] private ViewReference introView;

    [SerializeField] private LocalData localData;
    [SerializeField] private Tutorial tutorial;
    [SerializeField] private Controller controller;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private FungalController fungalPrefab;
    [SerializeField] private Transform fungalSpawnPosition;

    private GameObject currentFungal;

    private void Awake()
    {
        viewController.OnFadeInStart += () =>
        {
            var fungal = Instantiate(fungalPrefab, fungalSpawnPosition.position, Quaternion.LookRotation(Vector3.back + Vector3.right));
            fungal.Initialize(fungalInventory.Fungals[0], isGrove: false);
            currentFungal = fungal.gameObject;

            // todo: shouldn't need to reference controller
            controller.SetMovement(fungal.Movement);
        };

        partyButton.onClick.AddListener(() =>
        {
            navigation.Navigate(matchmakingView);
        });

        adventureButton.onClick.AddListener(() =>
        {
            if (tutorial.IsCompleted)
            {
                sceneNavigation.NavigateToScene(2);
            }
            else
            {
                sceneNavigation.NavigateToScene(1);
            }
        });

        resetButton.onClick.AddListener(() =>
        {
            localData.ResetData();
            if (currentFungal) Destroy(currentFungal);
            navigation.Navigate(introView);
        });
    }

}
