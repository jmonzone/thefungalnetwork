using System.Collections;
using Cinemachine;
using UnityEngine;

public class AllIsBog : MonoBehaviour
{
    [SerializeField] private GameReference game;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CinemachineVirtualCamera arenaCamera;

    [SerializeField] private FadeCanvasGroup canvasGroup;

    [SerializeField] private AbilityButton fishButton;
    [SerializeField] private AbilityButton dashButton;
    [SerializeField] private FishingRodUI fishingRodUI;
    [SerializeField] private MoveCharacterJoystick moveCharacterJoystick;

    private void Awake()
    {
        game.Initialize();
        if (!Application.isEditor) canvasGroup.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        game.OnClientPlayerAdded += PlayerReference_OnClientPlayerAdded;
        game.OnAllPlayersAdded += PlayerReference_OnPlayerUpdated;
    }

    private void OnDisable()
    {
        game.OnClientPlayerAdded -= PlayerReference_OnClientPlayerAdded;
        game.OnAllPlayersAdded -= PlayerReference_OnPlayerUpdated;
    }

    private void PlayerReference_OnClientPlayerAdded()
    {
        var fungalThrow = game.ClientPlayer.Fungal.GetComponent<FungalThrow>();
        fishingRodUI.AssignFishingRod(fungalThrow);
        fishButton.AssignAbility(fungalThrow);

        var fungalDash = game.ClientPlayer.Fungal.GetComponent<FungalDash>();
        dashButton.AssignAbility(fungalDash);
        fungalDash.OnDashStart += () => moveCharacterJoystick.enabled = false;
        fungalDash.OnDashComplete += () => moveCharacterJoystick.enabled = true;
        
    }

    private void PlayerReference_OnPlayerUpdated()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        if (!Application.isEditor)
        {
            yield return new WaitForSeconds(2f);
            yield return canvasGroup.FadeOut();
        }
        arenaCamera.Priority = 0;
        cameraController.Target = game.ClientPlayer.Fungal.transform;
        yield return new WaitForSeconds(1f);
        game.StartGame();
    }
}