using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class IngredientManager : MonoBehaviour
{
    [SerializeField] private float maxLaunchForce;
    [SerializeField] private float minLaunchForce;
    [SerializeField] private float throwFrequency;
    [SerializeField] private float maxHorizontal;
    [SerializeField] private float maxLaunchAngle;

    private ObjectPool ingredientPool;

    private void Start()
    {
        ingredientPool = GetComponent<ObjectPool>();
        ingredientPool.OnSpawn += obj =>
        {
            var ingredient = obj.GetComponent<IngredientController>();
            var launchAngle = Quaternion.Euler(0, 0, Random.Range(-maxLaunchAngle, maxLaunchAngle)) * Vector3.up;
            ingredient.RigidBody.AddForce(launchAngle * Random.Range(minLaunchForce, maxLaunchForce));
        };
    }

    public void SpawnIngredients()
    {
        ingredientPool.enabled = true;
    }

    public void StopIngredients()
    {
        ingredientPool.enabled = false;
    }

}
