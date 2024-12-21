using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomObjective : MonoBehaviour
{
    [SerializeField] private Controller controller;

    private MovementController movement;
    private MovementController mountedController;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
    }

    private void Update()
    {
        if (mountedController)
        {
            mountedController.transform.position = transform.position + Vector3.up * 0.625f;
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
            if (playerRb.velocity.y <= 0)
            {
                Debug.Log("Player landed on this object!");
                Mount();
            }
        }
    }

    private void Mount()
    {
        mountedController = controller.Movement;
        controller.SetMovement(movement);
    }
}
