using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PufferfishExplosion : MonoBehaviour
{
    [SerializeField] private GameObject render;

    public event UnityAction OnExplodeComplete;

    public void DealExplosionDamage(float damage, float radius = 1f)
    {
        // Detect all colliders, including triggers
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hitColliders)
        {
            var fungal = hit.GetComponent<NetworkFungal>();
            if (fungal != null)
            {
                fungal.ModifySpeedServerRpc(0, 0.5f);
                fungal.TakeDamageServerRpc(damage);
                continue;
            }

            var bubble = hit.GetComponentInParent<Bubble>();
            if (bubble != null)
            {
                bubble.PopServerRpc();
                continue;
            }
        }
    }


    public void StartExplosionAnimation(float radius = 1f)
    {
        render.transform.localScale = 2f * radius * Vector3.one;
        StartCoroutine(ExplosionRoutine());
    }

    public IEnumerator ExplosionRoutine()
    {
        render.SetActive(true);
        OnExplodeComplete?.Invoke();
        yield return new WaitForSeconds(0.5f);
        render.SetActive(false);
    }
}
