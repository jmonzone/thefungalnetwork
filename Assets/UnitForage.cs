using System.Collections;
using UnityEngine;

public class UnitForage : MonoBehaviour
{
    private UnitMovement movement;

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
    }

    public void StartForage(Forageable forageable)
    {
        Debug.Log("StartForage");
        StopAllCoroutines();
        StartCoroutine(ForageRoutine(forageable));
    }

    private IEnumerator ForageRoutine(Forageable forageable)
    {
        var direction = forageable.transform.position - transform.position;
        var targetPosition = forageable.transform.position - direction.normalized * 1f;
        movement.StartMovement(targetPosition);
        yield return null;


    }
}
