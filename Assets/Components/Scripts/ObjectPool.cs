using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPool<T> where T : MonoBehaviour
{
    public List<T> Objects = new List<T>();

    private int index;

    public ObjectPool(T prefab, Transform parent, UnityAction<T> onInstantiate)
    {
        Objects = new List<T>();

        for (var i = 0; i < 10; i++)
        {
            var obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            onInstantiate(obj);
            Objects.Add(obj);
        }
    }

    public T Spawn(Vector3 position)
    {
        var obj = GetNextObject();
        obj.transform.position = position;
        obj.gameObject.SetActive(true);

        return obj;
    }

    private T GetNextObject()
    {
        var i = 0;
        while(i < Objects.Count)
        {
            var obj = Objects[index % Objects.Count];
            index++;
            i++;

            if (obj.gameObject.activeSelf) continue;
            else return obj;
        }
        return Objects[index % Objects.Count];
    }

}

public abstract class ObjectPoolManager<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private float frequency;
    [SerializeField] private int maxObjects;
    [SerializeField] protected PositionAnchor spawnPosition;

    private Dictionary<T,ObjectPool<T>> Pools = new Dictionary<T, ObjectPool<T>>();
    private float timer;

    private void Awake()
    {
        foreach (var prefab in Prefabs)
        {
            var pool = new ObjectPool<T>(prefab, transform, OnInstantiate);
            Pools.Add(prefab, pool);
        }
    }

    private void OnDisable()
    {
        foreach(var pool in Pools.Keys)
        {
            foreach(var obj in Pools[pool].Objects)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (timer > frequency)
        {
            var pool = GetTargetPool(Pools);

            if (Pools.All(pool => pool.Value.Objects.Where(obj => obj.gameObject.activeSelf).Count() < maxObjects))
            {
                var obj = pool.Spawn(spawnPosition.Position);
                OnSpawn(obj);

                timer = 0;
            }
        }

        timer += Time.deltaTime;
    }

    protected abstract List<T> Prefabs { get; }
    protected abstract void OnInstantiate(T obj);
    protected abstract void OnSpawn(T obj);
    protected abstract ObjectPool<T> GetTargetPool(Dictionary<T, ObjectPool<T>> pools);
}
