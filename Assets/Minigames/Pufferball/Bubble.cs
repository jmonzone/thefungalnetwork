using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bubble : NetworkBehaviour
{
    [SerializeField] private float autoPopTime = 3f;
    [SerializeField] private float popDuration = 0.3f;
    [SerializeField] private float damage = 3f;

    private Movement movement;
    private NetworkVariable<bool> canHit = new NetworkVariable<bool>(true);  // Use NetworkVariable
    private bool hitRequested = false;

    private Material bubbleMaterial;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        movement = GetComponent<Movement>();
        bubbleMaterial = GetComponentInChildren<Renderer>().material;

        if (IsOwner)
        {
            StartCoroutine(InflateRoutine());
        }
    }

    private IEnumerator InflateRoutine()
    {
        yield return movement.ScaleOverTime(0.75f, 0, 1f);
        yield return new WaitForSeconds(autoPopTime);
        PopServerRpc();
    }

    private void Update()
    {
        if (IsOwner && canHit.Value)  // Check the NetworkVariable's value
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
                fungal.TakeDamageServerRpc(damage);
                hitRequested = true;

                PopServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PopServerRpc()
    {
        if (canHit.Value) PopClientRpc();
    }

    [ClientRpc]
    private void PopClientRpc()
    {
        if (canHit.Value)  // Check the NetworkVariable's value
        {
            StopAllCoroutines();
            StartCoroutine(PopAnimation());
        }

        if (IsOwner && canHit.Value)
        {
            // Update canHit value on the server to false when pop happens
            SetCanHitServerRpc(false);
            hitRequested = false;
        }
    }

    private IEnumerator PopAnimation()
    {
        float elapsedTime = 0; // Sync timing
        float startIntensity = bubbleMaterial.GetFloat("_Intensity");
        Color startColor = bubbleMaterial.GetColor("_Outer_Color");

        float peakScale = 1.4f;
        float endScale = 0f;
        float duration = popDuration;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float popCurve = Mathf.SmoothStep(0f, 1f, t);

            // Expand first, then shrink
            float scaleFactor = (t < 0.5f)
                ? Mathf.Lerp(1f, peakScale, t * 2f)
                : Mathf.Lerp(peakScale, endScale, (t - 0.5f) * 2f);

            movement.SetScaleFactor(scaleFactor);

            // Modify shader properties
            bubbleMaterial.SetFloat("_Intensity", Mathf.Lerp(startIntensity, startIntensity * 0.5f, popCurve));
            bubbleMaterial.SetColor("_Outer_Color", new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0f, popCurve)));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final values
        movement.SetScaleFactor(0f);
        movement.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        bubbleMaterial.SetFloat("_Intensity", startIntensity);
        bubbleMaterial.SetColor("_Outer_Color", startColor);
    }

    // ServerRpc to update canHit on all clients
    [ServerRpc]
    private void SetCanHitServerRpc(bool value)
    {
        canHit.Value = value;  // Update NetworkVariable on the server
    }
}
