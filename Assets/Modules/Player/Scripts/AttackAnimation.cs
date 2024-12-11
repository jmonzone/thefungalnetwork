using UnityEngine;

[RequireComponent(typeof(Attackable))]
public class AttackAnimation : MonoBehaviour
{
    [SerializeField] private Gradient hitColor;
    [SerializeField] private float hitColorDuration = 2f;
    [SerializeField] private float flashDuration = 0.5f; // Time for flash to complete

    private Gradient defaultColor;

    private void Awake()
    {
        var attackable = GetComponent<Attackable>();

        var particles = GetComponentInChildren<ParticleSystem>();
        if (particles)
        {
            defaultColor = particles.colorOverLifetime.color.gradient;
            var particleColorManager = GetComponent<ParticleColorManager>();

            attackable.OnCurrentHealthChanged += () =>
            {
                particleColorManager.ChangeColor(hitColor, hitColorDuration, () =>
                {
                    particleColorManager.ChangeColor(defaultColor, hitColorDuration);
                });
            };
        }
       
        var movementController = GetComponent<MovementController>();
        var materialFlasher = GetComponent<MaterialFlasher>();

        attackable.OnCurrentHealthChanged += () =>
        {
            if (attackable.CurrentHealth <= 0)
            {
                GetComponentInChildren<Animator>().Play("Death");
                movementController.Stop();
            }
            else
            {
                GetComponentInChildren<Animator>().Play("Hit");
            }

            materialFlasher.FlashWhite();
        };
    }
}
