using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientManager : MonoBehaviour
{
    [SerializeField] private IngredientController ingredientPrefab;
    [SerializeField] private Transform ingredientSpawnAnchor;
    [SerializeField] private float maxLaunchForce;
    [SerializeField] private float minLaunchForce;
    [SerializeField] private float throwFrequency;
    [SerializeField] private float maxHorizontal;
    [SerializeField] private float maxLaunchAngle;

    private List<IngredientController> ingredientPool = new List<IngredientController>();
    private int ingredientIndex;

    private void Start()
    {
        for(var i = 0; i < 10; i++)
        {
            var ingredient = Instantiate(ingredientPrefab);
            ingredient.gameObject.SetActive(false);
            ingredientPool.Add(ingredient);
        }

        StartCoroutine(ThrowIngredients());
    }

    private IEnumerator ThrowIngredients()
    {
        while (true)
        {
            yield return new WaitForSeconds(throwFrequency);

            var spawnPostion = ingredientSpawnAnchor.position;
            spawnPostion.x = Random.Range(-maxHorizontal, maxHorizontal);

            var ingredient = ingredientPool[ingredientIndex % ingredientPool.Count];
            ingredient.Spawn(spawnPostion);

            var launchAngle = Quaternion.Euler(0, 0, Random.Range(-maxLaunchAngle, maxLaunchAngle)) * Vector3.up;
            ingredient.RigidBody.AddForce(launchAngle * Random.Range(minLaunchForce, maxLaunchForce));

            ingredientIndex++;
        }
    }
}
