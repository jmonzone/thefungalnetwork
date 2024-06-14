using UnityEngine;

public class IngredientManager : ObjectPool<IngredientController>
{
    [SerializeField] private float maxLaunchForce;
    [SerializeField] private float minLaunchForce;
    [SerializeField] private float throwFrequency;
    [SerializeField] private float maxHorizontal;
    [SerializeField] private float maxLaunchAngle;

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
