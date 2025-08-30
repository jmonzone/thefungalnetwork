using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPickup : MonoBehaviour
{
    private void Update()
    {
        var colliders = Physics.OverlapBox(transform.position, Vector3.one * 0.5f);

        foreach(var collider in colliders)
        {
            var sporeController = collider.GetComponentInParent<SporeController>();
            if (sporeController)
            {
                sporeController.Collect();
            }
        }
    }
}
