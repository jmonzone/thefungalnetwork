using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private T prefab;
    [SerializeField] private float frequency;
    [SerializeField] protected PositionAnchor spawnPosition;

    private List<T> pool = new List<T>();
    private int index;
    private float timer;

    private void Awake()
    {
        for (var i = 0; i < 10; i++)
        {
            var obj = Instantiate(prefab);
            obj.gameObject.SetActive(false);
            OnInstantiate(obj);
            pool.Add(obj);
        }
    }

    private void Update()
    {
        if (timer > frequency)
        {
            var obj = pool[index % pool.Count];
            obj.transform.position = spawnPosition.Position;
            obj.gameObject.SetActive(true);
            OnSpawn(obj);

            timer = 0;
            index++;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    protected abstract void OnInstantiate(T obj);
    protected abstract void OnSpawn(T obj);
}
