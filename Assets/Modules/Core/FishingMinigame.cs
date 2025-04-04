using System.Collections;
using Cinemachine;
using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
    [SerializeField] private GameReference playerReference;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CinemachineVirtualCamera arenaCamera;

    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference inputView;
    [SerializeField] private FadeCanvasGroup canvasGroup;

    [SerializeField] private AbilityButton fishButton;
    [SerializeField] private AbilityButton dashButton;
    [SerializeField] private FishingRodUI fishingRodUI;
    [SerializeField] private MoveCharacterJoystick moveCharacterJoystick;

    //[SerializeField] private GameObject scoreText;

    private void Awake()
    {
        if (!Application.isEditor) canvasGroup.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        playerReference.OnClientPlayerAdded += PlayerReference_OnClientPlayerAdded;
        playerReference.OnAllPlayersAdded += PlayerReference_OnPlayerUpdated;
    }

    private void PlayerReference_OnClientPlayerAdded()
    {
        playerReference.OnClientPlayerAdded -= PlayerReference_OnClientPlayerAdded;
        var fungalThrow = playerReference.ClientPlayer.Fungal.GetComponent<FungalThrow>();
        fishingRodUI.AssignFishingRod(fungalThrow);
        fishButton.AssignAbility(fungalThrow);

        var fungalDash = playerReference.ClientPlayer.Fungal.GetComponent<FungalDash>();
        dashButton.AssignAbility(fungalDash);
        fungalDash.OnDashStart += () => moveCharacterJoystick.enabled = false;
        fungalDash.OnDashComplete += () => moveCharacterJoystick.enabled = true;
        
    }

    private void OnDisable()
    {
        playerReference.OnAllPlayersAdded -= PlayerReference_OnPlayerUpdated;
    }

    private void PlayerReference_OnPlayerUpdated()
    {
        playerReference.OnAllPlayersAdded -= PlayerReference_OnPlayerUpdated;

        StartCoroutine(WaitForSeconds());
    }

    private IEnumerator WaitForSeconds()
    {
        if (!Application.isEditor)
        {
            yield return new WaitForSeconds(2f);
            yield return canvasGroup.FadeOut();
        }
        arenaCamera.Priority = 0;
        cameraController.Target = playerReference.ClientPlayer.Fungal.transform;
        yield return new WaitForSeconds(1f);
        navigation.Navigate(inputView);

    }
}