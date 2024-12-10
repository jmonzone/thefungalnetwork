using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocodileAttack : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float hitCooldown = 3f;
    [SerializeField] private float hitTimer = 0;

    private MovementController movementController;

    // Start is called before the first frame update
    void Start()
    {
        movementController = GetComponent<MovementController>();
        //movementController.SetTarget(target);
    }

    // Update is called once per frame
    void Update()
    {
        if (movementController.IsAtDestination && hitTimer > hitCooldown)
        {
            GetComponentInChildren<Animator>().Play("Attack");
            hitTimer = 0;
            direction = target.transform.position - movementController.transform.position;
        }

        hitTimer += Time.deltaTime;
    }
}
