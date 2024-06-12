using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float frequency;
    [SerializeField] private Transform ingredientSpawnAnchor;


    private List<GameObject> ingredientPool = new List<GameObject>();
    private int ingredientIndex;

    public event UnityAction<GameObject> OnObjectInstantiated;

    private void Awake()
    {
        for (var i = 0; i < 10; i++)
        {
            var ingredient = Instantiate(prefab);
            ingredient.SetActive(false);
            ingredientPool.Add(ingredient);
        }
    }

    private float timer;

    private void Update()
    {
        if (timer > frequency)
        {
            var ingredient = ingredientPool[ingredientIndex % ingredientPool.Count];
            ingredientIndex++;

            var spawnPostion = ingredientSpawnAnchor.position;
            ingredient.transform.position = spawnPostion;
            ingredient.gameObject.SetActive(true);

            OnObjectInstantiated?.Invoke(ingredient);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

}
