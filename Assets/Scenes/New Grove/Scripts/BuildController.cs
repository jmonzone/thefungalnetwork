using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BuildController : MonoBehaviour
{
    [SerializeField] private Vector3 overlapHalfExtents = new Vector3(0.5f, 0.5f, 0.5f);

    private Camera mainCamera;

    private List<Material> defaultMaterials;
    private List<Renderer> renderers;
    private Collider objectCollider;

    const float moveSpeed = 15f; // Lerp speed

    public event UnityAction OnBuildComplete;

    private void Awake()
    {
        mainCamera = Camera.main;

        renderers = GetComponentsInChildren<Renderer>().ToList();
        objectCollider = GetComponentInChildren<Collider>();

        defaultMaterials = new List<Material>();

        // Apply color to all child renderers' materials
        foreach (var rend in renderers)
        {
            var material = rend.materials[0]; // Copies instances
            defaultMaterials.Add(material);
        }
    }

    public void StartBuild(LayerMask placementMask, LayerMask collisionMask, Material validMaterial, Material invalidMaterial)
    {
        StopAllCoroutines();
        StartCoroutine(BuildRoutine(placementMask, collisionMask, validMaterial, invalidMaterial));
    }

    private IEnumerator BuildRoutine(LayerMask placementMask, LayerMask collisionMask, Material validMaterial, Material invalidMaterial)
    {
        // Immediately place at a valid start position
        Vector3 targetPosition = GetPlacementPosition(placementMask);
        transform.position = targetPosition;

        while (true)
        {
            // Calculate new target position
            targetPosition = GetPlacementPosition(placementMask);

            // Smooth movement
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

            // Check collision validity
            bool isValid = IsPlacementValid(targetPosition, placementMask, collisionMask);

            var i = 0;
            // Apply color to all child renderers' materials
            foreach (var rend in renderers)
            {
                rend.material = isValid ? validMaterial : invalidMaterial;
                i++;
            }

            yield return null;
        }
    }

    private Vector3 GetPlacementPosition(LayerMask placementMask)
    {
        Vector3 screenPoint = new Vector3(Screen.width / 2f, Screen.height / 2f + 25f, 0f);
        Ray ray = mainCamera.ScreenPointToRay(screenPoint);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementMask))
        {
            return hit.point;
        }

        // If no ground hit, fallback to horizontal plane
        Plane horizontalPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));
        if (horizontalPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return transform.position;
    }

    private bool IsPlacementValid(Vector3 position, LayerMask placementMask, LayerMask collisionMask)
    {
        bool prevState = objectCollider.enabled;
        objectCollider.enabled = false;

        bool onGround = Physics.Raycast(
            position + Vector3.up * 0.5f,
            Vector3.down,
            1f,
            placementMask
        );

        bool blocked = Physics.CheckBox(
            position,
            overlapHalfExtents,
            transform.rotation,
            collisionMask,
            QueryTriggerInteraction.Ignore
        );

        objectCollider.enabled = prevState;

        return onGround && !blocked;
    }


    public void CompleteBuild()
    {
        StopAllCoroutines();

        var i = 0;

        // Apply color to all child renderers' materials
        foreach (var rend in renderers)
        {
            rend.material = defaultMaterials[i];
            var material = rend.materials[0];
            i++;
        }
        OnBuildComplete?.Invoke();
    }
}
