using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniFrog : MonoBehaviour
{
    private Transform spore;
    private Transform targetPosition;
    private MovementController movement;
    private MountController mountController;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
        mountController = GetComponent<MountController>();
        mountController.OnMounted += MountController_OnMounted;
    }

    private void MountController_OnMounted()
    {
        movement.Stop();
        mountController.Mountable.Movement.SetPosition(targetPosition.position);
    }

    private void Update()
    {
        if (mountController && mountController.HasMount.Value && targetPosition && Vector3.Distance(transform.position, targetPosition.position) < 2f)
        {
            movement.Jump();
        }
    }

    public void AssignSpore(Transform spore, Transform targetPosition)
    {
        this.spore = spore;
        this.targetPosition = targetPosition;
        movement.SetTarget(spore);
    }
}
