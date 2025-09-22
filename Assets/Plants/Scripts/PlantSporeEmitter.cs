using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.Events;

public class ObjectPool<T> where T : Component
{
    private T prefab;
    private Transform parent;
    private Queue<T> pool = new Queue<T>();

    public ObjectPool(T prefab, int initialSize = 10, Transform parent = null, UnityAction<T> initialize = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        // Pre-instantiate objects
        for (int i = 0; i < initialSize; i++)
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
            initialize?.Invoke(obj);
        }
    }

    public T Get()
    {
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            T obj = Object.Instantiate(prefab, parent);
            return obj;
        }
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}


public class PlantSporeEmitter : MonoBehaviour, IInteractable, INoteTarget
{
    [Header("References")]
    [SerializeField] private SporeController sporePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private DJTableReference dJTableReference;
    [SerializeField] private Transform scaleTransform;

    [Header("Settings")]
    [SerializeField] private int emissionStep = 8;
    [SerializeField] private float launchHeight = 2f;
    [SerializeField] private float bounceScale = 1.2f;
    [SerializeField] private float bounceDuration = 0.3f;

    public int EmissionStep => emissionStep;
    Transform ITarget.Transform => transform;

    private Vector3 startScale;
    private ObjectPool<SporeController> sporePool;

    private void Awake()
    {
        startScale = scaleTransform.localScale;
        sporePool = new ObjectPool<SporeController>(sporePrefab, 10, transform, spore =>
        {
            spore.OnCollect += () => sporePool.Return(spore);
        });
    }

    void IInteractable.Select()
    {
        EmitSpore();
    }

    public void EmitSpore()
    {
        StopAllCoroutines();
        StartCoroutine(BounceAnimation());

        SporeController spore = sporePool.Get();
        spore.transform.position = spawnPoint.position;
        spore.transform.rotation = Quaternion.identity;

        Vector3 peak = spawnPoint.position + Vector3.up * launchHeight;
        Vector3 landingSpot = FindLandingSpot();

        spore.LaunchSpore(peak, landingSpot);
    }

    private IEnumerator BounceAnimation()
    {
        scaleTransform.localScale = startScale;

        Vector3 targetScale = startScale * bounceScale;
        float t = 0f;

        // Scale up
        while (t < bounceDuration)
        {
            t += Time.deltaTime;
            float progress = t / bounceDuration;
            scaleTransform.localScale = Vector3.Lerp(startScale, targetScale, Mathf.Sin(progress * Mathf.PI));
            yield return null;
        }

        scaleTransform.localScale = startScale;
    }

    private Vector3 FindLandingSpot()
    {
        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        randomDirection.z = randomDirection.y;
        randomDirection.y = 0;
        randomDirection *= Random.Range(1.25f, 1.75f);

        Vector3 targetPos = transform.position + randomDirection;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        Debug.Log("fallback");
        // fallback: just drop next to plant
        return transform.position + (Vector3.right * 1f);
    }

    void INoteTarget.OnHit(DJTrack track)
    {
        EmitSpore();
    }

    void IInteractable.OnProximityChanged(bool value)
    {
    }
}
