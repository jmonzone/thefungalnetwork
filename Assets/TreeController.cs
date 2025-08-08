using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private Animator eyeballAnimator;

    private void Awake()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        eyeballAnimator = GetComponentInChildren<Animator>(true);
    }
    public void OnSelect()
    {
        virtualCamera.Priority = 11;
        eyeballAnimator.enabled = true;
    }
}
