using System.Collections;
using System.Linq;
using UnityEngine;

public class CrocodileAI : MonoBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private float hitCooldown = 3f;
    [SerializeField] private float hitTimer = 0;

    private AbilityCast abilityCast;
    private Vector3 direction;
    private Attackable attackable;

    private void Awake()
    {
        abilityCast = GetComponent<AbilityCast>();

        attackable = GetComponent<Attackable>();
        attackable.OnHealthDepleted += Attackable_OnHealthDepleted;
    }

    private void Attackable_OnHealthDepleted()
    {
        StopAllCoroutines();
        enabled = false;
    }

    private void OnEnable()
    {
        abilityCast.OnCastComplete += AbilityCast_OnCast;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        abilityCast.OnCastComplete -= AbilityCast_OnCast;
    }

    private void AbilityCast_OnCast()
    {
        if (attackable.CurrentHealth == 0) return;
        hitTimer = 0;
        StartCoroutine(Start());
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() =>
        {
            hitTimer += Time.deltaTime;
            return hitTimer > hitCooldown;
        });

        var targets = arena.Players;

        yield return new WaitUntil(() =>
        {
            targets = arena.Players
                  .Where(player => Vector3.Distance(player.transform.position, transform.position) < 5f)
                  .OrderBy(player => Vector3.Distance(player.transform.position, transform.position))
                  .ToList();
            return targets.Count > 0;
        });

        var target = targets[0];

        direction = target.position - transform.position;
        direction.y = 0;
        abilityCast.SetDirection(direction.normalized);
        abilityCast.Prepare();
        yield return new WaitForSeconds(1.75f);
        abilityCast.StartCast();

    }
}
