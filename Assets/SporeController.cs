using System.Collections;
using UnityEngine;

public class SporeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryReference inventoryReference;

    [Header("Spore Settings")]
    [SerializeField] private float driftDownDuration = 2f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    private void DetectClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var sporeController = hit.collider.GetComponentInParent<SporeController>();
            if (this == sporeController)
            {
                OnSporeClicked();
            }
        }
    }

    private void OnSporeClicked()
    {
        inventoryReference.IncreaseSporeCount(1);

        StopAllCoroutines();

        //todo: pool
        Destroy(gameObject);
    }

    public void LaunchSpore(Vector3 peak, Vector3 landing)
    {
        StartCoroutine(AnimateSpore(peak, landing));
    }

    private IEnumerator AnimateSpore(Vector3 peak, Vector3 landing)
    {
        Vector3 start = transform.position;
        float t = 0;

        // Go upward
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, peak, t / 0.5f);
            yield return null;
        }

        // Drift down with side sway like feather
        t = 0;
        while (t < driftDownDuration)
        {
            t += Time.deltaTime;
            float progress = t / driftDownDuration;

            Vector3 pos = Vector3.Lerp(peak, landing, progress);
            pos.x += Mathf.Sin(progress * Mathf.PI * 2f) * 0.2f; // feather sway
            transform.position = pos;

            yield return null;
        }

        transform.position = landing;
    }
}
