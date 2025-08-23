using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UnitForage : MonoBehaviour
{
    [SerializeField] private float forageSpeed = 2f;

    private UnitMovement movement;

    public event UnityAction<Forageable> OnUnitHasForaged;

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
        movement.OnIsMovingHasChanged += Movement_OnIsMovingHasChanged;
    }

    private void Movement_OnIsMovingHasChanged(bool isMoving)
    {
        if (isMoving) StopAllCoroutines();
    }

    public void StartForage(Forageable forageable)
    {
        Debug.Log("StartForage");
        StopAllCoroutines();

        var direction = forageable.transform.position - transform.position;
        var targetPosition = forageable.transform.position - direction.normalized * 1f;
        movement.StartMovement(targetPosition, () =>
        {
            StartCoroutine(ForageRoutine(forageable));
        });
    }

    private IEnumerator ForageRoutine(Forageable forageable)
    {
        yield return new WaitForFixedUpdate();

        while (true)
        {
            OnUnitHasForaged?.Invoke(forageable);
            yield return new WaitForSeconds(forageSpeed);
        }
    }
}
