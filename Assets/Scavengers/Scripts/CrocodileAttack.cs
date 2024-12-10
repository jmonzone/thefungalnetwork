using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocodileAttack : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float hitCooldown = 3f;
    [SerializeField] private float hitTimer = 0;
    [SerializeField] private AbilityCast abilityCast;

    private MovementController movementController;

    // Start is called before the first frame update
    void Start()
    {
        movementController = GetComponent<MovementController>();
        abilityCast.StartCast(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (movementController.IsAtDestination && hitTimer > hitCooldown)
        {
            GetComponentInChildren<Animator>().Play("Attack");
            hitTimer = 0;
        }

        direction = target.position - movementController.transform.position;
        direction.y = 0;
        abilityCast.UpdateCast(direction.normalized);

        hitTimer += Time.deltaTime;
    }
}
