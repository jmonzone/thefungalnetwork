using System.Collections;
using Cinemachine;
using UnityEngine;


public class Grove : MonoBehaviour
{
    [SerializeField] private FadeCanvasGroup canvasGroup;
    [SerializeField] private CinemachineVirtualCamera arenaCamera;
    [SerializeField] private CameraController cameraController;

    [SerializeField] private GameObject fungal;

    private void Start()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        yield return canvasGroup.FadeOut();

        arenaCamera.Priority = 0;
        cameraController.Target = fungal.transform;
    }
}
