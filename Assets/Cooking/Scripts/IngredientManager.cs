using System.Collections.Generic;
using UnityEngine;

public class IngredientManager : ObjectPoolManager<IngredientController>
{
    [SerializeField] private IngredientController prefab;
    [SerializeField] private float maxLaunchForce;
    [SerializeField] private float minLaunchForce;
    [SerializeField] private float throwFrequency;
    [SerializeField] private float maxHorizontal;
    [SerializeField] private float maxLaunchAngle;

    protected override List<IngredientController> Prefabs => new List<IngredientController> { prefab };

    protected override ObjectPool<IngredientController> GetTargetPool(Dictionary<IngredientController, ObjectPool<IngredientController>> pools)
    {
        return pools[prefab];
    }

    protected override void OnInstantiate(IngredientController obj)
    {

    }

    protected override void OnSpawn(IngredientController obj)
    {
        var ingredient = obj.GetComponent<IngredientController>();
        ingredient.Reset();

        var launchAngle = Quaternion.Euler(0, 0, Random.Range(-maxLaunchAngle, maxLaunchAngle)) * Vector3.up;
        ingredient.RigidBody.AddForce(launchAngle * Random.Range(minLaunchForce, maxLaunchForce));
    }
}
