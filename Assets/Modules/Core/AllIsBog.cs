using System.Collections;
using Cinemachine;
using UnityEngine;

public class AllIsBog : MonoBehaviour
{
    [SerializeField] private GameReference game;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CinemachineVirtualCamera arenaCamera;

    [SerializeField] private FadeCanvasGroup canvasGroup;

    [Header("UI")]
    [SerializeField] private FungalThrow fungalThrow;
    [SerializeField] private AbilityButton fishButton;
    [SerializeField] private AbilityButton fungalAbilityButton;
    [SerializeField] private FishingRodUI fishingRodUI;
    [SerializeField] private MoveCharacterJoystick moveCharacterJoystick;

    private void Awake()
    {
        game.Initialize();
        //if (!Application.isEditor)
        canvasGroup.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        game.OnClientDataInitialized += UpdateUIAbilityButtons;
        game.OnAllPlayersAdded += OnAllPlayersAdded;
    }

    private void OnDisable()
    {
        game.OnClientDataInitialized -= UpdateUIAbilityButtons;
        game.OnAllPlayersAdded -= OnAllPlayersAdded;
    }

    // todo: centralize ability instance cration with fungalAI
    private void UpdateUIAbilityButtons()
    {
        var throwAbility = Instantiate(fungalThrow);
        throwAbility.Initialize(game.ClientPlayer.Fungal);

        Debug.Log($"PlayerReference_OnClientPlayerAdded {game.ClientPlayer.Fungal.Data.Id} {game.ClientPlayer.Fungal.Data.Ability.Id}");

        fishingRodUI.AssignFishingRod(throwAbility);
        fishButton.AssignAbility(throwAbility);

        var abilityTemplate = game.ClientPlayer.Fungal.Data.Ability;
        var fungalAbility = Instantiate(abilityTemplate);
        fungalAbility.Initialize(game.ClientPlayer.Fungal);

        if (fungalAbility is IMovementAbility movementAbility)
        {
            fungalAbility.OnAbilityStart += () => moveCharacterJoystick.enabled = false;
            fungalAbility.OnAbilityComplete += () => moveCharacterJoystick.enabled = true;
        }

        fungalAbilityButton.AssignAbility(fungalAbility);

    }

    private void OnAllPlayersAdded()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        //if (!Application.isEditor)
        //{
            yield return new WaitForSeconds(1f);
            yield return canvasGroup.FadeOut();
        //}
        arenaCamera.Priority = 0;
        cameraController.Target = game.ClientPlayer.Fungal.transform;
        yield return new WaitForSeconds(1f);
        game.StartGame();
    }
}