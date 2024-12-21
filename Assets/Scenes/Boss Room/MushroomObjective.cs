using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//todo: centralize with crocodile interaction
public class MushroomObjective : MonoBehaviour
{
    [SerializeField] private Controller controller;

    private MovementController movement;
    private MovementController mountedController;

    public event UnityAction OnMounted;
    public event UnityAction OnUnmounted;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
        movement.OnJump += Unmount;
    }

    private void Update()
    {
        if (mountedController)
        {
            mountedController.transform.position = transform.position + Vector3.up * 0.625f;
            mountedController.LookAt(controller.Movement.Direction);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        if (mountedController) return;
        Rigidbody playerRb = collision.rigidbody;

        var movement = collision.gameObject.GetComponentInParent<MovementController>();

        // Ensure the player is the one colliding
        if (movement && movement == controller.Movement)
        {
            Debug.Log("contacts");
            // Check if the player is falling onto the object
            if (playerRb.velocity.y < 0)
            {
                Debug.Log("Player landed on this object!");
                Mount();
            }
        }
    }

    private void Mount()
    {
        mountedController = controller.Movement;
        mountedController.GetComponent<ProximityAction>().SetInteractable(false);
        controller.SetMovement(movement);
        OnMounted?.Invoke();
    }

    private void Unmount()
    {
        mountedController.GetComponent<ProximityAction>().SetInteractable(true);
        controller.SetMovement(mountedController);
        movement.Stop();
        mountedController = null;
        OnUnmounted?.Invoke();
    }
}
