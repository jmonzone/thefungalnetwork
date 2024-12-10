using System.Collections;
using UnityEngine;

public class CrocodileAttack : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float hitCooldown = 3f;
    [SerializeField] private float hitTimer = 0;
    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDistance = 10f;

    private Vector3 startPosition;
    private MovementController movementController;

    // Start is called before the first frame update
    private void Start()
    {
        startPosition = transform.position;
        movementController = GetComponent<MovementController>();
        StartCoroutine(AttackTimer());
    }

    private IEnumerator AttackTimer()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitUntil(() =>
            {
                hitTimer += Time.deltaTime;
                return hitTimer > hitCooldown;
            });
            yield return new WaitUntil(() => movementController.IsAtDestination);
            yield return AimAttack();
        }
    }

    private IEnumerator AimAttack()
    {
        abilityCast.StartCast(transform);

        var elapsedTime = 0f;
        while (elapsedTime < 2)
        {
            direction = target.position - movementController.transform.position;
            direction.y = 0;
            abilityCast.UpdateCast(direction.normalized);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        abilityCast.EndCast();
        GetComponentInChildren<Animator>().Play("Attack");

        movementController.SetSpeed(chargeSpeed);
        var targetPosition = abilityCast.StartPosition + abilityCast.Direction * chargeDistance;
        movementController.SetPosition(targetPosition);
        yield return new WaitUntil(() => movementController.IsAtDestination);
        movementController.SetSpeed(chargeSpeed * 0.75f);
        movementController.SetPosition(startPosition);

        yield return new WaitUntil(() => movementController.IsAtDestination);
        movementController.Stop();

        transform.position = startPosition;
        transform.forward = Vector3.back;
        hitTimer = 0;
    }
}
