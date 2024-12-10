using System.Collections;
using UnityEngine;

public class Crocodile : MonoBehaviour
{
    [SerializeField] private GameObject overheadUI;
    [SerializeField] private Gradient hitColor;
    [SerializeField] private float hitColorDuration = 2f;
    [SerializeField] private float flashDuration = 0.5f; // Time for flash to complete

    private Attackable attackable;
    private MovementController movementController;
    private ParticleColorManager particleColorManager;
    private MaterialFlasher materialFlasher;
    private Gradient defaultColor;

    private HealthSlider healthSlider;

    private void Awake()
    {
        overheadUI.SetActive(true);

        movementController = GetComponent<MovementController>();

        healthSlider = GetComponentInChildren<HealthSlider>(includeInactive: true);

        var particles = GetComponentInChildren<ParticleSystem>();
        defaultColor = particles.colorOverLifetime.color.gradient;

        particleColorManager = GetComponent<ParticleColorManager>();
        materialFlasher = GetComponent<MaterialFlasher>();
    }

    // todo: attack attackable, crocodile listens
    public void Damage()
    {
        attackable.Damage();

        if (attackable.CurrentHealth == 0)
        {
            GetComponentInChildren<Animator>().Play("Death");
            movementController.Stop();
        }
        else
        {
            GetComponentInChildren<Animator>().Play("Hit");
        }

        particleColorManager.ChangeColor(hitColor, hitColorDuration, () =>
        {
            particleColorManager.ChangeColor(defaultColor, hitColorDuration);
        });

        materialFlasher.FlashWhite();
    }
}
