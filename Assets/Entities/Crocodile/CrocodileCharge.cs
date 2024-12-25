using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocodileCharge : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float chargeForce;

    private Rigidbody rb;
    private AbilityCast abilityCast;

    private Attackable attackable;

    private bool isAttacking = false;
    private List<Attackable> attackables = new List<Attackable>();

    private void Awake()
    {
        abilityCast = GetComponent<AbilityCast>();
        rb = GetComponent<Rigidbody>();

        attackable = GetComponent<Attackable>();
        attackable.OnHealthDepleted += StopAllCoroutines;
    }

    private void OnEnable()
    {
        abilityCast.OnCastStart += ChargeAttack;
    }

    private void OnDisable()
    {
        abilityCast.OnCastStart -= ChargeAttack;
    }

    private void ChargeAttack()
    {
        StartCoroutine(ChargeAttackRoutine());
    }

    private IEnumerator ChargeAttackRoutine()
    {
        if (attackable.CurrentHealth == 0) yield break;
        GetComponentInChildren<Animator>().Play("Attack");

        rb.velocity = abilityCast.Direction.normalized * chargeForce;
        transform.forward = abilityCast.Direction.normalized;
        isAttacking = true;
        yield return new WaitUntil(() => rb.velocity.magnitude < 10f);
        isAttacking = false;
        attackables = new List<Attackable>();
        abilityCast.CompleteCast();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isAttacking) return;

        var attackable = collision.transform.GetComponentInParent<Attackable>();
        if (attackable && !attackables.Contains(attackable))
        {
            Debug.Log($"Collided {collision.transform.name}");
            attackable.DamageServerRpc(damage);
            attackables.Add(attackable);
        }
    }
}
