using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EggController : MonoBehaviour
{
    [SerializeField] private ProximityAction proximityAction;

    public FungalData Fungal { get; private set; }

    private Rigidbody _rigidbody;

    public event UnityAction OnHatch;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Initialize(FungalData fungal)
    {
        Fungal = fungal;
        var renderer = GetComponentInChildren<Renderer>();
        renderer.material.color = fungal.EggColor;

        if (proximityAction)
        {
            proximityAction.Sprite = Fungal.ActionImage;
            proximityAction.Color = Fungal.ActionColor;
            proximityAction.OnUse += Hatch;
        }
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
