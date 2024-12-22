using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniFrog : MonoBehaviour
{
    private Transform spore;
    private MovementController movement;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
    }

    public void AssignSpore(Transform spore)
    {
        this.spore = spore;
        movement.SetTarget(spore);
    }
}
