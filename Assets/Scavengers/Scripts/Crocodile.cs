using System.Collections;
using UnityEngine;

public class Crocodile : MonoBehaviour
{
    [SerializeField] private GameObject overheadUI;
    [SerializeField] private Gradient hitColor;
    [SerializeField] private float hitColorDuration = 2f;
    [SerializeField] private float flashDuration = 0.5f; // Time for flash to complete
    [SerializeField] private Transform target;

    private MovementController movementController;
    private ParticleColorManager particleColorManager;
    private MaterialFlasher materialFlasher;
    private Gradient defaultColor;

    private HealthSlider healthSlider;

    private void Awake()
    {
        overheadUI.SetActive(true);

        movementController = GetComponent<MovementController>();
        movementController.SetTarget(target);

        healthSlider = GetComponentInChildren<HealthSlider>(includeInactive: true);

        var particles = GetComponentInChildren<ParticleSystem>();
        defaultColor = particles.colorOverLifetime.color.gradient;

        particleColorManager = GetComponent<ParticleColorManager>();
        materialFlasher = GetComponent<MaterialFlasher>();
    }

    private float hitCooldown = 1.5f;
    private float hitTimer = 0;

    private void Update()
    {
        if (movementController.IsAtDestination && hitTimer > hitCooldown)
        {
            GetComponentInChildren<Animator>().Play("Hit");
            hitTimer = 0;
        }

        hitTimer += Time.deltaTime;
        
    }

    public void Damage()
    {
        healthSlider.Damage();

        if (healthSlider.Value == 0)
        {
            GetComponentInChildren<Animator>().Play("Death");
            movementController.Stop();
        }

        particleColorManager.ChangeColor(hitColor, hitColorDuration, () =>
        {
            particleColorManager.ChangeColor(defaultColor, hitColorDuration);
        });

        materialFlasher.FlashWhite();
    }
}
