using System.Collections;
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
    private void Start()
    {
        movementController = GetComponent<MovementController>();
        StartCoroutine(AttackRoutine());
    }

    // Update is called once per frame
    private void Update()
    {
        hitTimer += Time.deltaTime;
    }

    private IEnumerator AttackRoutine()
    {
        while (gameObject.activeSelf)
        {
            yield return Attack();
        }
    }

    private IEnumerator Attack()
    {
        yield return new WaitUntil(() => hitTimer > hitCooldown);
        yield return new WaitUntil(() => movementController.IsAtDestination);

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

        abilityCast.EndCast();
        GetComponentInChildren<Animator>().Play("Attack");
        hitTimer = 0;
    }
}
