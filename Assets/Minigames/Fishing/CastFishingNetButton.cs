using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CastFishingNetButton : MonoBehaviour
{
    [SerializeField] private AutoTargetReticle autoTargetReticle;
    [SerializeField] private Button castFishingNetButton;
    [SerializeField] private Transform fishingNet;
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float cooldown = 2f;

    private bool isOnCooldown = false;

    private void Awake()
    {
        castFishingNetButton.onClick.AddListener(() => StartCoroutine(ThrowNet()));
    }

    private void Update()
    {
        castFishingNetButton.interactable = autoTargetReticle.TargetFishController && !isOnCooldown;
    }

    private IEnumerator ThrowNet()
    {
        if (isOnCooldown) yield break;

        isOnCooldown = true;
        castFishingNetButton.interactable = false; // Disable button

        var targetFish = autoTargetReticle.TargetFishController;
        if (!targetFish) yield break;

        fishingNet.gameObject.SetActive(true);

        var start = playerReference.Transform.position + (targetFish.transform.position - playerReference.Transform.position).normalized * 0.5f;
        var end = targetFish.transform.position;


        fishingNet.position = start;

        var xzDirection = end - start;
        xzDirection.y = 0;
        xzDirection.Normalize();

        float elapsed = 0f;
        float maxHeight = 2f; // Adjust this for the peak height of the toss

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration; // Progress from 0 to 1

            // Interpolate position in XZ plane
            var currentXZPosition = Vector3.Lerp(start, end, t);

            // Calculate Y based on parabolic equation
            float y = Mathf.Lerp(start.y, end.y, t) + maxHeight * (1 - 4 * (t - 0.5f) * (t - 0.5f)); // Parabola

            // Update position
            fishingNet.position = new Vector3(currentXZPosition.x, y, currentXZPosition.z);
            fishingNet.Rotate(Vector3.down, Time.deltaTime);

            yield return null;
        }

        // Ensure the net reaches the exact target position
        fishingNet.transform.position = end;
        targetFish.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);

        fishingNet.gameObject.SetActive(false);
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
}
