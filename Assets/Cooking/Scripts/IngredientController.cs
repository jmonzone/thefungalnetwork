using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientController : MonoBehaviour
{
    [SerializeField] private GameObject whole;
    [SerializeField] private GameObject sliced;

    public Rigidbody RigidBody { get; private set; }
    private Collider ingredientCollider;

    private void Awake()
    {
        RigidBody = GetComponent<Rigidbody>();
        ingredientCollider = GetComponent<Collider>();
    }

    public void Spawn(Vector3 position)
    {
        transform.SetPositionAndRotation(position, Random.rotation);
        gameObject.SetActive(true);

        whole.SetActive(true);
        sliced.SetActive(false);
        ingredientCollider.enabled = true;

        RigidBody.velocity = Vector3.zero;

        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody slice in slices)
        {
            slice.transform.localPosition = Vector3.zero;
            slice.velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        BladeController blade = other.GetComponentInParent<BladeController>();
        if (blade)
        {
            Slice(blade.Direciton, blade.transform.position, blade.sliceForce);
        }
    }

    private void Slice(Vector3 direction, Vector3 position, float force)
    {
        whole.SetActive(false);
        sliced.SetActive(true);
        ingredientCollider.enabled = false;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sliced.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        foreach(Rigidbody slice in slices)
        {
            slice.velocity = RigidBody.velocity;
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
        }
    }
}
