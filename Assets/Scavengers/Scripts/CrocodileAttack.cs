using System.Collections;
using UnityEngine;

public class CrocodileAttack : MonoBehaviour
{
    [SerializeField] private Controller target;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float hitCooldown = 3f;
    [SerializeField] private float hitTimer = 0;
    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDistance = 10f;

    private Vector3 startPosition;
    private MovementController movementController;
    private Attackable attackable;
    private Coroutine attackCoroutine;

    // Start is called before the first frame update
    private void Start()
    {
        startPosition = transform.position;
        movementController = GetComponent<MovementController>();
        attackable = GetComponent<Attackable>();
        attackable.OnDeath += () => StopCoroutine(attackCoroutine);

        attackCoroutine = StartCoroutine(AttackTimer());
    }

    private IEnumerator AttackTimer()
    {
        do
        {
            yield return new WaitUntil(() =>
            {
                hitTimer += Time.deltaTime;
                return hitTimer > hitCooldown;
            });

            yield return new WaitUntil(() => movementController.IsAtDestination);
            yield return AimAttack();
        }
        while (attackable.CurrentHealth > 0 && target.Attackable.CurrentHealth > 0);
    }

    private IEnumerator AimAttack()
    {
        abilityCast.StartCast(transform, attackable => true);

        var elapsedTime = 0f;
        while (elapsedTime < 2)
        {
            direction = target.Movement.transform.position - transform.position;
            direction.y = 0;
            abilityCast.UpdateCast(direction.normalized);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // give the player a chance to react
        yield return new WaitForSeconds(0.75f);

        abilityCast.Cast();
        GetComponentInChildren<Animator>().Play("Attack");

        movementController.SetSpeed(chargeSpeed);
        var targetPosition = abilityCast.StartPosition + abilityCast.Direction * chargeDistance;
        movementController.SetPosition(targetPosition);
        isAttacking = true;
        yield return new WaitUntil(() => movementController.IsAtDestination);
        isAttacking = false;
        movementController.SetSpeed(chargeSpeed * 0.75f);
        movementController.SetPosition(startPosition);

        yield return new WaitUntil(() => movementController.IsAtDestination);
        movementController.Stop();

        transform.position = startPosition;
        transform.forward = Vector3.back;
        hitTimer = 0;
    }

    private bool isAttacking = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        var attackable = other.transform.GetComponentInParent<Attackable>();
        if (attackable)
        {
            Debug.Log($"Collided {other.transform.name}");
            attackable.Damage();
        }
    }
}
