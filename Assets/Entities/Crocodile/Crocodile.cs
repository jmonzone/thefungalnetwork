using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crocodile : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float hitCooldown = 3f;
    [SerializeField] private float hitTimer = 0;
    [SerializeField] private AbilityCastReference abilityCast;
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDistance = 10f;
    [SerializeField] private SceneNavigation sceneNavigation;

    private MovementController movementController;
    private Attackable attackable;

    private void OnEnable()
    {
        sceneNavigation.OnSceneNavigationRequest += StopAllCoroutines;
        Target_OnUpdate();
    }

    private void OnDisable()
    {
        sceneNavigation.OnSceneNavigationRequest -= StopAllCoroutines;
        controller.OnUpdate -= Target_OnUpdate;
    }

    private void Target_OnUpdate()
    {
        StopAllCoroutines();
        StartCoroutine(AttackTimer());
    }

    // Start is called before the first frame update
    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        attackable = GetComponent<Attackable>();
        attackable.OnHealthDepleted += StopAllCoroutines;
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
        while (attackable.CurrentHealth > 0 && controller.Attackable.CurrentHealth > 0);
    }

    private IEnumerator AimAttack()
    {
        yield return new WaitUntil(() => arena.Players.Count > 0);

        var target = arena.Players
            .OrderBy(player => Vector3.Distance(player.transform.position, transform.position))
            .FirstOrDefault();

        direction = target.position - transform.position;
        direction.y = 0;
        abilityCast.StartCast(transform, direction.normalized, attackable => true);

        yield return new WaitForSeconds(1.5f);

        abilityCast.Cast();
        GetComponentInChildren<Animator>().Play("Attack");

        movementController.SetSpeed(chargeSpeed);
        var targetPosition = abilityCast.StartPosition + abilityCast.Direction * chargeDistance;
        movementController.SetPosition(targetPosition);
        isAttacking = true;
        yield return new WaitUntil(() => movementController.IsAtDestination);
        isAttacking = false;
        attackables = new List<Attackable>();
        movementController.Stop();
        hitTimer = 0;
    }

    private bool isAttacking = false;

    private List<Attackable> attackables = new List<Attackable>();

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
