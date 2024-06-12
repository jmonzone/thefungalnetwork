using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float frequency;
    [SerializeField] private PositionAnchor spawnPosition;

    private List<GameObject> pool = new List<GameObject>();
    private int index;

    public event UnityAction<GameObject> OnInstantiate;
    public event UnityAction<GameObject> OnSpawn;

    private void Awake()
    {
        for (var i = 0; i < 10; i++)
        {
            var obj = Instantiate(prefab);
            obj.SetActive(false);
            OnInstantiate?.Invoke(obj);
            pool.Add(obj);
        }
    }

    private float timer;

    private void Update()
    {
        if (timer > frequency)
        {
            var obj = pool[index % pool.Count];
            index++;

            obj.transform.position = spawnPosition.Position;
            obj.SetActive(true);

            OnSpawn?.Invoke(obj);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }


}
