using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PufferfishExplosion : MonoBehaviour
{
    [SerializeField] private GameObject render;

    public event UnityAction OnExplodeComplete;

    public void Explode(float radius = 1f)
    {
        render.transform.localScale = Vector3.one * radius;
        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        render.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        render.SetActive(false);
        OnExplodeComplete?.Invoke();
    }
}
