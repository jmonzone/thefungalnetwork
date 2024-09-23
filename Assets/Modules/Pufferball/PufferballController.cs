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
        transform.position = new Vector3(0, 3, 0);
        Rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(true);

    }
}
