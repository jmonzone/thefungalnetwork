using UnityEngine;

public class PufferballController :  MonoBehaviour
{
    public Rigidbody Rigidbody { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();

        Spawn();
    }

    public void Spawn()
    {
        Debug.Log("spawning");
        transform.position = new Vector3(0, 3, 0);
        gameObject.SetActive(true);

        var targetDirection = Random.onUnitSphere;
        targetDirection.y = 0;

        Rigidbody.velocity = 5f * targetDirection.normalized;
    }
}
