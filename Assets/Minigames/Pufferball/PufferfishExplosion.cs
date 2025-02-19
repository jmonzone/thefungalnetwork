using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PufferfishExplosion : MonoBehaviour
{
    [SerializeField] private GameObject render;

    public event UnityAction OnExplodeComplete;

    public void DealExplosionDamage(float radius = 1f)
    {
        // Detect all players in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hitColliders)
        {
            var fungal = hit.GetComponent<NetworkFungal>();
            if (fungal != null)
            {
                fungal.TakeDamageServerRpc(1f);
                break;
            }
        }
    }

    public void StartExplosionAnimation(float radius = 1f)
    {
        render.transform.localScale = Vector3.one * radius;
        StartCoroutine(ExplosionRoutine());
    }

    public IEnumerator ExplosionRoutine()
    {
        render.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        render.SetActive(false);
        OnExplodeComplete?.Invoke();
    }
}
