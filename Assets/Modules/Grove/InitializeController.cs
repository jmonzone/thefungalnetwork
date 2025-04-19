using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class InitializeController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private FungalThrow fungalThrow;
    [SerializeField] private AbilityButton fishButton;
    [SerializeField] private AbilityButton fungalAbilityButton;
    [SerializeField] private FishingRodUI fishingRodUI;
    [SerializeField] private MoveCharacterJoystick moveCharacterJoystick;

    [Header("Views")]
    [SerializeField] private FadeCanvasGroup loadingCanvas;
    [SerializeField] private CinemachineVirtualCamera arenaCamera;
    [SerializeField] private CameraController cameraController;

    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference inputView;

    private FungalController fungal;

    private void Start()
    {
        Debug.Log($"InitializeController.Start");
        loadingCanvas.gameObject.SetActive(true);
    }

    public void Initialize(FungalController fungal, UnityAction onComplete = null)
    {
        Debug.Log($"InitializeController.Initialize");
        this.fungal = fungal;
        StartCoroutine(InitializeRoutine(onComplete));
    }

    private IEnumerator InitializeRoutine(UnityAction onComplete)
    {
        Debug.Log($"InitializeController.InitializeRoutine");

        yield return new WaitForSeconds(1f);
        Debug.Log($"InitializeController.InitializeRoutine.WaitForSeconds");
        yield return loadingCanvas.FadeOut();
        Debug.Log($"InitializeController.InitializeRoutine.FadeOut");


        arenaCamera.Priority = 0;
        cameraController.Target = fungal.transform;

        UpdateUIAbilityButtons();

        yield return new WaitForSeconds(1f);

        navigation.Navigate(inputView);

        onComplete?.Invoke();
    }

    // todo: centralize ability instance cration with fungalAI
    private void UpdateUIAbilityButtons()
    {
        var throwAbility = Instantiate(fungalThrow);
        throwAbility.Initialize(fungal);

        Debug.Log($"PlayerReference_OnClientPlayerAdded {fungal.Data.Id} {fungal.Data.Ability.Id}");

        fishingRodUI.AssignFishingRod(throwAbility);
        fishButton.AssignAbility(throwAbility);

        var abilityTemplate = fungal.Data.Ability;
        var fungalAbility = Instantiate(abilityTemplate);
        fungalAbility.Initialize(fungal);

        if (fungalAbility is IMovementAbility movementAbility)
        {
            fungalAbility.OnAbilityStart += () => moveCharacterJoystick.enabled = false;
            fungalAbility.OnAbilityComplete += () => moveCharacterJoystick.enabled = true;
        }

        fungalAbilityButton.AssignAbility(fungalAbility);
    }
}
