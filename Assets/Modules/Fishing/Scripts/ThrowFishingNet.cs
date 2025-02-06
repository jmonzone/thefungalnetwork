using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowFishingNet : MonoBehaviour
{
    [SerializeField] private Button throwButton;
    [SerializeField] private float duration;


    [SerializeField] private Transform reticle;
    [SerializeField] private Transform net;
    [SerializeField] private Transform netStartPosition;

    private void Start()
    {
        throwButton.onClick.AddListener(() => StartCoroutine(ThrowNet()));
    }

    private IEnumerator ThrowNet()
    {
        throwButton.interactable = false;

        net.gameObject.SetActive(true);
        reticle.gameObject.SetActive(false);

        var start = net.transform.position;
        var end = reticle.transform.position;

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
            net.transform.position = new Vector3(currentXZPosition.x, y, currentXZPosition.z);
            net.Rotate(Vector3.down, Time.deltaTime * 250f);

            yield return null;
        }

        // Ensure the net reaches the exact target position
        net.transform.position = end;

        var sphereCast = Physics.OverlapSphere(reticle.transform.position, reticle.transform.localScale.x);
        //var fishControllers = sphereCast.Select(collider => collider.GetComponentInParent<FishController>()).OfType<FishController>().ToList();
        //foreach (var fish in fishControllers)
        //{
        //    fish.gameObject.SetActive(false);
        //    OnFishCaught?.Invoke();
        //}

        //inventory.AddToInventory(fishData, fishControllers.Count);

        //inventoryText.text = inventory.GetItemCount(fishData).ToString();
        yield return new WaitForSeconds(0.25f);

        net.transform.position = netStartPosition.position;

        throwButton.interactable = true;
        reticle.gameObject.SetActive(true);

    }
}
