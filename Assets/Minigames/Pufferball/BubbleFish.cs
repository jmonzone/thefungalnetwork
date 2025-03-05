using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class BubbleFish : NetworkBehaviour
{
    [SerializeField] private float autoPopTime = 3f;
    [SerializeField] private float popDuration = 0.3f; // Duration of pop animation

    private Fish fish;
    private bool canHit = false;
    private bool hitRequested = false;
    private Material bubbleMaterial;

    private void Awake()
    {
        fish = GetComponent<Fish>();
        bubbleMaterial = GetComponentInChildren<Renderer>().material; // Get shader material

        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowComplete += ThrowFish_OnThrowComplete;
    }

    private void ThrowFish_OnThrowComplete()
    {
        StartCoroutine(AutoPopTimer());
    }

    private IEnumerator AutoPopTimer()
    {
        yield return fish.Movement.ScaleOverTime(0.75f, 1f, 1.75f);
        canHit = true;

        yield return new WaitForSeconds(autoPopTime);
        PopServerRpc();
    }

    private void Update()
    {
        if (IsOwner && canHit)
        {
            CheckPlayerHit();
        }
    }

    private void CheckPlayerHit()
    {
        if (hitRequested) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (Collider hit in hitColliders)
        {
            var fungal = hit.GetComponent<NetworkFungal>();
            if (fungal != null)
            {
                fungal.ModifySpeedServerRpc(0f, 1.5f);
                fungal.TakeDamageServerRpc(1f);
                hitRequested = true;

                PopServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PopServerRpc()
    {
        Debug.Log("bubble.PopServerRpc()");
        if (canHit) PopClientRpc();
    }

    [ClientRpc]
    private void PopClientRpc()
    {
        if (IsOwner && canHit)
        {
            StopAllCoroutines();
            StartCoroutine(PopAnimation());
            canHit = false;
            hitRequested = false;
        }
    }

    private IEnumerator PopAnimation()
    {
        float elapsedTime = 0f;
        float startIntensity = bubbleMaterial.GetFloat("_Intensity");

        Color startColor = bubbleMaterial.GetColor("_Outer_Color");

        float peakScale = 1.4f; // Slight expansion before popping
        float endScale = 0f;     // Fully disappears
        float duration = popDuration;

        // Parallel Scaling and Shader Updates
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float popCurve = Mathf.SmoothStep(0f, 1f, t);

            // Expand first, then shrink
            float scaleFactor = (t < 0.5f)
                ? Mathf.Lerp(1f, peakScale, t * 2f)
                : Mathf.Lerp(peakScale, endScale, (t - 0.5f) * 2f);

            // Modify scale via movement script if available
            fish.Movement.SetScaleFactor(scaleFactor); // Call method to modify scale


            // Modify shader properties
            bubbleMaterial.SetFloat("_Intensity", Mathf.Lerp(startIntensity, startIntensity * 0.5f, popCurve));
            bubbleMaterial.SetColor("_Outer_Color", new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0f, popCurve)));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final values
        fish.Movement.SetScaleFactor(0f); // Call method to modify scale

        yield return new WaitForSeconds(1f);

        bubbleMaterial.SetFloat("_Intensity", startIntensity);
        bubbleMaterial.SetColor("_Outer_Color", startColor);

        fish.ReturnToRadialMovement();
    }


}
