using System.Collections;
using Cinemachine;
using UnityEngine;

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

    private void Awake()
    {
        loadingCanvas.gameObject.SetActive(true);
    }

    public void Initialize(FungalController fungal)
    {
        this.fungal = fungal;
        StartCoroutine(InitializeRoutine());
    }

    private IEnumerator InitializeRoutine()
    {
        yield return new WaitForSeconds(1f);
        yield return loadingCanvas.FadeOut();

        arenaCamera.Priority = 0;
        cameraController.Target = fungal.transform;

        UpdateUIAbilityButtons();

        yield return new WaitForSeconds(1f);

        navigation.Navigate(inputView);

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
