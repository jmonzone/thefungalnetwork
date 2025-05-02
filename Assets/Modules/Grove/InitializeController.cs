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
        if (fungal.InnateAbility is IMovementAbility)
        {
            fungal.InnateAbility.OnAbilityStart += () => moveCharacterJoystick.enabled = false;
            fungal.InnateAbility.OnAbilityComplete += () => moveCharacterJoystick.enabled = true;
        }

        fungalAbilityButton.AssignAbility(fungal.InnateAbility);

        if (fungal.ExternalAbility)
        {
            if (fungal.ExternalAbility is IMovementAbility)
            {
                fungal.ExternalAbility.OnAbilityStart += () => moveCharacterJoystick.enabled = false;
                fungal.ExternalAbility.OnAbilityComplete += () => moveCharacterJoystick.enabled = true;
            }

            fungalThrowButton.AssignAbility(fungal.ExternalAbility);
        }

    }

    private void OnAbilityChanged()
    {
        Debug.Log("OnAbilityChanged" + fungal.ExternalAbility?.name);
        //fungalThrowUI.enabled = false;
        fungalThrowButton.AssignAbility(fungal.ExternalAbility);
    }
}
