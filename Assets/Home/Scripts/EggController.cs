using UnityEngine;

public class EggController : MonoBehaviour
{
    public PetData Pet { get; private set; }

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Jump()
    {
        rigidbody.AddForce(Vector3.up * 100f);
    }

    public void SetPet(PetData pet)
    {
        Pet = pet;
        var renderer = GetComponentInChildren<Renderer>();
        renderer.material.color = pet.Color;
    }
}
