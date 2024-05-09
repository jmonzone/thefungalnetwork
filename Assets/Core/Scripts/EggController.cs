using UnityEngine;

public class EggController : MonoBehaviour
{
    public Pet Pet { get; private set; }

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Jump()
    {
        _rigidbody.AddForce(Vector3.up * 100f);
    }

    public void SetPet(Pet pet)
    {
        Pet = pet;
        var renderer = GetComponentInChildren<Renderer>();
        renderer.material.color = pet.Color;
    }
}
