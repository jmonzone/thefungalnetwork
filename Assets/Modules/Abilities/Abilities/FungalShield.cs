using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/Ability/Shield")]
public class FungalShield : Ability
{
    [SerializeField] private float shieldPower = 2f; // duration in seconds
    [SerializeField] private float shieldDuration = 5f; // duration in seconds

    private Coroutine shieldTimerCoroutine;

    public override void Initialize(FungalController fungal)
    {
        base.Initialize(fungal);
    }

    public override void CastAbility()
    {
        base.CastAbility();

        Fungal.ToggleShieldRenderers(true);
        Fungal.Health.SetShield(shieldPower);

        Fungal.Health.OnShieldChanged += Health_OnShieldChanged;

        if (shieldTimerCoroutine != null) Fungal.StopCoroutine(shieldTimerCoroutine);

        shieldTimerCoroutine = Fungal.StartCoroutine(ShieldDurationTimer(shieldDuration));
    }

    private IEnumerator ShieldDurationTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Shield still active? Expire it
        if (Fungal.Health.CurrentShield > 0)
        {
            Fungal.Health.SetShield(0f); // This will trigger OnShieldChanged
        }
    }

    private void Health_OnShieldChanged()
    {
        if (Fungal.Health.CurrentShield == 0)
        {
            Fungal.ToggleShieldRenderers(false);
            CompleteAbility();

            Fungal.Health.OnShieldChanged -= Health_OnShieldChanged;

            if (shieldTimerCoroutine != null)
            {
                Fungal.StopCoroutine(shieldTimerCoroutine);
                shieldTimerCoroutine = null;
            }
        }
    }
}
