using System.Collections;
using UnityEngine;

public class BuildController : MonoBehaviour
{
    [SerializeField] private Vector3 overlapHalfExtents = new Vector3(0.5f, 0.5f, 0.5f);

    private Camera mainCamera;

    private Material defaultMaterial;
    private Renderer objectRenderer;
    private Collider objectCollider;

    private void Awake()
    {
        mainCamera = Camera.main;

        objectRenderer = GetComponentInChildren<Renderer>();
        objectCollider = GetComponentInChildren<Collider>();

        defaultMaterial = objectRenderer.material;
    }

    public void StartBuild(LayerMask placementMask, LayerMask collisionMask, Material validMaterial, Material invalidMaterial)
    {
        StopAllCoroutines();
        StartCoroutine(BuildRoutine(placementMask, collisionMask, validMaterial, invalidMaterial));
    }

    private IEnumerator BuildRoutine(LayerMask layerMask, LayerMask collisionMask, Material validMaterial, Material invalidMaterial)
    {
        while (true)
        {
            // Take the center of the screen and move it up by `screenYOffset` pixels
            Vector3 screenPoint = new Vector3(Screen.width / 2f, Screen.height / 2f + 25f, 0f);
            Ray ray = mainCamera.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                transform.position = hit.point;

                // Temporarily disable own collider to avoid self-hit
                bool prevState = objectCollider.enabled;
                objectCollider.enabled = false;

                bool blocked = Physics.CheckBox(
                    transform.position,
                    overlapHalfExtents,
                    transform.rotation,
                    collisionMask,
                    QueryTriggerInteraction.Ignore
                );

                objectCollider.enabled = prevState;

                // Change color based on viability
                objectRenderer.material = blocked ? invalidMaterial : validMaterial;
            }

            yield return null;
        }
    }

    public void CompleteBuild()
    {
        StopAllCoroutines();
        objectRenderer.material = defaultMaterial;
    }
}
