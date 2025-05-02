using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class InitializeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FadeCanvasGroup loadingCanvas;
    [SerializeField] private CinemachineVirtualCamera arenaCamera;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference inputView;

    [Header("UI")]
    [SerializeField] private MoveCharacterJoystick moveCharacterJoystick;
    [SerializeField] private AbilityButton fungalAbilityButton;
    [SerializeField] private AbilityButton fungalThrowButton;

    private FungalController fungal;

    private void Start()
    {
        loadingCanvas.gameObject.SetActive(true);
    }

    public void Initialize(FungalController fungal, UnityAction onComplete = null)
    {
        this.fungal = fungal;
        this.fungal.OnAbilityChanged += OnAbilityChanged;

        StartCoroutine(InitializeRoutine(onComplete));
    }

    private IEnumerator InitializeRoutine(UnityAction onComplete)
    {
        yield return new WaitForSeconds(1f);
        yield return loadingCanvas.FadeOut();

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

    private void OnAbilityChanged()
    {
        //fungalThrowUI.enabled = false;
        fungalThrowButton.AssignAbility(fungal.Ability);
    }
}
