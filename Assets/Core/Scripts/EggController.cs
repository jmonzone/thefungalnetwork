using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EggController : EntityController
{
    public Pet Fungal { get; private set; }

    private Rigidbody _rigidbody;

    public event UnityAction OnHatch;

    public override Sprite ActionImage => Fungal.ActionImage;
    public override Color ActionColor => Fungal.ActionColor;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Initialize(Pet fungal)
    {
        Fungal = fungal;
        var renderer = GetComponentInChildren<Renderer>();
        renderer.material.color = fungal.EggColor;
    }

    public void Hatch()
    {
        _rigidbody.AddForce(Vector3.up * 100f);
        StartCoroutine(OnEggHatched());
    }

    private IEnumerator OnEggHatched()
    {
        yield return new WaitForSeconds(1f);
        OnHatch?.Invoke();
        gameObject.SetActive(false);
    }
}
