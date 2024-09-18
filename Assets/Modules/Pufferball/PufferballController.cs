using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PufferballController :  NetworkBehaviour
{
    private MovementController movement;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        movement = GetComponent<MovementController>();

        var direction = new Vector3(-1, 0, 1);
        movement.SetDirection(direction);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Wall")
        {
            var contactPoint = collision.contacts[0];
            //movement.SetDirection()

            var direction = movement.Direction;
            //Debug.Log($"direction: {direction} normal: {contactPoint.normal}");
            Debug.Log($"z: {direction.z} normalz: {contactPoint.normal.z}");

            if (Mathf.Abs(contactPoint.normal.x) > 0.01f) direction.x *= -Mathf.Abs(contactPoint.normal.x);
            if (Mathf.Abs(contactPoint.normal.z) > 0.01f) direction.z *= -Mathf.Abs(contactPoint.normal.z);

            //Debug.Log($"direction: {direction} normal: {contactPoint.normal}");
            movement.SetDirection(direction);
            //var direction = new Vector3(mo, 0, 1);
            //movement.SetDirection(direction);
        }
    }
}
