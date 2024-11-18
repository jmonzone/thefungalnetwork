using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing : MonoBehaviour
{
    [SerializeField] private Transform reticle;

    private VirtualJoystick joystick;

    private void Awake()
    {
        joystick = GetComponentInChildren<VirtualJoystick>();
        joystick.OnJoystickUpdate += Joystick_OnJoystickUpdate;
    }

    private void Joystick_OnJoystickUpdate(Vector3 direction)
    {
        var translation = direction;
        translation.z = direction.y;
        translation.y = 0;

        reticle.transform.position += translation;
    }
}
