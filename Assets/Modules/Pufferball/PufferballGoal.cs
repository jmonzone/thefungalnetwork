using UnityEngine;

public class PufferballGoal : MonoBehaviour
{
    [SerializeField] private LayerMask pufferballLayer;

    private void Update()
    {
        var colliders = Physics.OverlapSphere(transform.position, 1, pufferballLayer);
        if (colliders.Length > 0)
        {
            Debug.Log("Goal!");
            var pufferball = colliders[0].GetComponentInParent<PufferballController>();
            pufferball.gameObject.SetActive(false);
            pufferball.Spawn();
        }
    }
}
