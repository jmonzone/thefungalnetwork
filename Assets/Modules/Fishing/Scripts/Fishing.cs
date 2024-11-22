using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fishing : MonoBehaviour
{
    [Header("Input References")]
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private Button interactionButton;
    [SerializeField] private TextMeshProUGUI inventoryText;

    [Header("Rendering References")]
    [SerializeField] private Transform reticle;
    [SerializeField] private Transform reticleStartPosition;
    [SerializeField] private Transform net;
    [SerializeField] private Transform netStartPosition;
    [SerializeField] private float duration;

    [Header("Game References")]
    [SerializeField] private FishData fishData;
    [SerializeField] private ItemInventory inventoryService;
    [SerializeField] private ViewReference fishingView;

    private void Awake()
    {
        joystick.OnJoystickUpdate += Joystick_OnJoystickUpdate;
        interactionButton.onClick.AddListener(() => StartCoroutine(ThrowNet()));

        fishingView.OnOpened += () =>
        {
            reticle.position = reticleStartPosition.transform.position;
            reticle.gameObject.SetActive(true);
        };

        fishingView.OnClosed += () =>
        {
            reticle.gameObject.SetActive(false);
        };
    }

    private void Start()
    {
        inventoryText.text = inventoryService.GetItemCount(fishData).ToString();
    }

    private void Joystick_OnJoystickUpdate(Vector3 direction)
    {
        var translation = direction;
        translation.z = direction.y;
        translation.y = 0;

        reticle.transform.position += translation;
    }

    private IEnumerator ThrowNet()
    {
        interactionButton.interactable = false;
        net.transform.position = netStartPosition.position;
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
        var fishControllers = sphereCast.Select(collider => collider.GetComponentInParent<FishController>()).OfType<FishController>().ToList();
        foreach (var fish in fishControllers)
        {
            fish.gameObject.SetActive(false);
        }

        inventoryService.AddToInventory(fishData, fishControllers.Count);

        inventoryText.text = inventoryService.GetItemCount(fishData).ToString();
        yield return new WaitForSeconds(0.25f);

        interactionButton.interactable = true;
        net.gameObject.SetActive(false);
        reticle.gameObject.SetActive(true);

    }
}
