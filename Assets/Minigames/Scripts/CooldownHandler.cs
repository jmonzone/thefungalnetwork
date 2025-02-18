using UnityEngine;
using System.Collections;

public class CooldownHandler : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 2f; // Set default cooldown time
    private bool isOnCooldown;

    public float CooldownTimer => cooldownTime;
    public bool IsOnCooldown => isOnCooldown;

    public void StartCooldown()
    {
        if (!isOnCooldown)
        {
            StartCoroutine(CooldownRoutine());
        }
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}
