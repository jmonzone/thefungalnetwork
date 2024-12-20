using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrocodileCharge : MonoBehaviour
{
    private MovementController movement;
    private AbilityCast abilityCast;

    private bool isAttacking = false;
    private List<Attackable> attackables = new List<Attackable>();

    // Start is called before the first frame update
    private void Awake()
    {
        abilityCast = GetComponent<AbilityCast>();
        abilityCast.OnCastStart += ChargeAttack;

        movement = GetComponent<MovementController>();
    }

    private void ChargeAttack()
    {
        StartCoroutine(ChargeAttackRoutine());
    }

    private IEnumerator ChargeAttackRoutine()
    {
        GetComponentInChildren<Animator>().Play("Attack");

        movement.SetSpeed(abilityCast.Data.Speed);
        var targetPosition = abilityCast.StartPosition + abilityCast.Direction * abilityCast.Data.MaxDistance;
        movement.SetPosition(targetPosition);
        isAttacking = true;

        yield return new WaitUntil(() => movement.IsAtDestination);

        isAttacking = false;
        attackables = new List<Attackable>();
        movement.SetSpeed(2f);
        movement.Stop();
        abilityCast.CompleteCast();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isAttacking) return;

        Debug.Log("onTriggerEnter");
        var attackable = collision.transform.GetComponentInParent<Attackable>();
        if (attackable && !attackables.Contains(attackable))
        {
            Debug.Log($"Collided {collision.transform.name}");
            attackable.Damage();
            attackables.Add(attackable);
        }
    }
}
