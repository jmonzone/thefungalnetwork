using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NetworkFungal : NetworkBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private MovementController movement;

    public float pulseIntensity = 0.8f; // Peak intensity of the vignette during the pulse
    public float pulseDuration = 0.3f; // Duration of each pulse
    public int pulseCount = 3; // Number of pulses

    public MovementController Movement => movement;

    private Vignette vignette;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        arena.RegisterPlayer(transform);

        var fungalController = GetComponent<FungalController>();
        fungalController.InitializeAnimations();
    }

    [ServerRpc]
    public void SetAsMinionServerRpc()
    {
        SetAsMinionClientRpc();
    }

    [ClientRpc]
    private void SetAsMinionClientRpc()
    {
        if (IsOwner)
        {
            Debug.Log("I am the minion");

                 // Try to get the Vignette effect from the volume
            if (controller.Volume.profile.TryGet<Vignette>(out var v))
            {
                vignette = v;
                StartCoroutine(PulseVignette());
            }
            else
            {
                Debug.LogError("Vignette not found in the volume profile!");
            }
        }

    }

    private IEnumerator PulseVignette()
    {
        if (vignette == null) yield break;

        float initialIntensity = vignette.intensity.value;
        for (int i = 0; i < pulseCount; i++)
        {
            // Pulse up
            yield return PulseToIntensity(pulseIntensity, pulseDuration / 2);

            // Pulse down
            yield return PulseToIntensity(initialIntensity, pulseDuration / 2);
        }
    }

    private IEnumerator PulseToIntensity(float targetIntensity, float duration)
    {
        float startIntensity = vignette.intensity.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, elapsed / duration);
            yield return null;
        }

        vignette.intensity.value = targetIntensity;
    }
}
