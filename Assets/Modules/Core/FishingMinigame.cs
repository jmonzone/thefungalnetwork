using System.Collections;
using Cinemachine;
using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
    [SerializeField] private PufferballReference playerReference;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CinemachineVirtualCamera arenaCamera;

    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference inputView;
    [SerializeField] private FadeCanvasGroup canvasGroup;

    //[SerializeField] private GameObject scoreText;

    private void Awake()
    {
        if (!Application.isEditor) canvasGroup.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        playerReference.OnAllPlayersAdded += PlayerReference_OnPlayerUpdated;
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
            yield return new WaitForSeconds(4f);
            yield return canvasGroup.FadeOut();
        }
        arenaCamera.Priority = 0;
        cameraController.Target = playerReference.ClientPlayer.Fungal.transform;
        yield return new WaitForSeconds(1f);
        navigation.Navigate(inputView);

    }
}