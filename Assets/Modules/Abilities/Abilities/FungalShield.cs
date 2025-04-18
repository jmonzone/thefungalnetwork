using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Fungals/Ability/Shield")]
public class FungalShield : Ability
{
    [SerializeField] private float shieldPower = 2f; // duration in seconds
    [SerializeField] private float shieldDuration = 5f; // duration in seconds

    private Coroutine shieldTimerCoroutine;

    public override void Initialize(NetworkFungal fungal)
    {
        base.Initialize(fungal);
    }

    public override void CastAbility()
    {
        base.CastAbility();

        fungal.ToggleShieldRenderers(true);
        fungal.Health.SetShield(shieldPower);

        fungal.Health.OnShieldChanged += Health_OnShieldChanged;

        if (shieldTimerCoroutine != null) fungal.StopCoroutine(shieldTimerCoroutine);

        shieldTimerCoroutine = fungal.StartCoroutine(ShieldDurationTimer(shieldDuration));
    }

    private IEnumerator ShieldDurationTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Shield still active? Expire it
        if (fungal.Health.CurrentShield > 0)
        {
            fungal.Health.SetShield(0f); // This will trigger OnShieldChanged
        }
    }

    private void Health_OnShieldChanged()
    {
        if (fungal.Health.CurrentShield == 0)
        {
            fungal.ToggleShieldRenderers(false);
            CompleteAbility();

            fungal.Health.OnShieldChanged -= Health_OnShieldChanged;

            if (shieldTimerCoroutine != null)
            {
                fungal.StopCoroutine(shieldTimerCoroutine);
                shieldTimerCoroutine = null;
            }
        }
    }
}
