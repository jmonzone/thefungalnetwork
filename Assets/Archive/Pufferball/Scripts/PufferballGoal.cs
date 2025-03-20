using UnityEngine;

public class PufferballGoal : MonoBehaviour
{
    [SerializeField] private float triggerRadius = 1f;
    [SerializeField] private LayerMask pufferballLayer;

    private void Update()
    {
        var colliders = Physics.OverlapSphere(transform.position, triggerRadius, pufferballLayer);
        if (colliders.Length > 0)
        {
            Debug.Log("Goal!");
            var pufferball = colliders[0].GetComponentInParent<PufferballController>();
            pufferball.gameObject.SetActive(false);
            pufferball.Spawn();
        }
    }
}
