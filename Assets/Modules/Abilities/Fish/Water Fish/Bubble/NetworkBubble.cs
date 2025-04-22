using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkBubble : NetworkBehaviour
{
    [SerializeField] private float autoPopTime = 3f;
    [SerializeField] private float popDuration = 0.3f;
    [SerializeField] private float inflateSpeed = 0.5f;
    [SerializeField] private float stunDuration = 2.25f;
    [SerializeField] private float damage = 3f;
    [SerializeField] private AudioClip audioClip;

    private Movement movement;
    private HitDetector hitDetector;
    private bool isPopped = false;

    private AudioSource audioSource;
    private Material bubbleMaterial;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        movement = GetComponent<Movement>();
        hitDetector = GetComponent<HitDetector>();
        audioSource = GetComponent<AudioSource>();
        bubbleMaterial = GetComponentInChildren<Renderer>().material;
    }

    private ulong sourceFungal;

    public void StartInflate(ulong sourceFungal)
    {
        this.sourceFungal = sourceFungal;
        StartCoroutine(InflateRoutine());
    }
    private IEnumerator InflateRoutine()
    {
        yield return movement.ScaleOverTime(inflateSpeed, 0, 1);
        yield return new WaitForSeconds(autoPopTime);
        Pop();
    }

    private void Update()
    {
        if (IsOwner && !isPopped)  // Check the NetworkVariable's value
        {
            hitDetector.CheckHits(movement.ScaleTransform.lossyScale.x / 2f, hit =>
            {
                var targetFungal = hit.GetComponent<FungalController>();

                if (targetFungal != null && !targetFungal.IsDead)
                {
                    targetFungal.ModifySpeed(0f, stunDuration, showStunAnimation: true);
                    targetFungal.Health.Damage(damage, sourceFungal);
                    Pop();
                }
            });
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PopServerRpc()
    {
        PopClientRpc();
    }

    [ClientRpc]
    private void PopClientRpc()
    {
        if (IsOwner) Pop();
    }

    private void Pop()
    {
        if (IsOwner)
        {
            isPopped = true;
            StopAllCoroutines();
            StartCoroutine(PopAnimation());
        }
    }

    private IEnumerator PopAnimation()
    {
        float elapsedTime = 0; // Sync timing
        float startIntensity = bubbleMaterial.GetFloat("_Intensity");
        Color startColor = bubbleMaterial.GetColor("_Base_Color");

        float peakScale = 1.4f;
        float endScale = 0f;
        float duration = popDuration;


        audioSource.clip = audioClip;
        audioSource.Play();

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
            bubbleMaterial.SetColor("_Base_Color", new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0f, popCurve)));

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        // Ensure final values
        movement.SetScaleFactor(0f);

        yield return new WaitForSeconds(1f);

        bubbleMaterial.SetFloat("_Intensity", startIntensity);
        bubbleMaterial.SetColor("_Base_Color", startColor);

        movement.gameObject.SetActive(false);
    }
}
