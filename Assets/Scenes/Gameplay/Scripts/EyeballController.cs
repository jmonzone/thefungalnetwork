using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeballController : MonoBehaviour
{
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float followDistance = 3f;   // Distance behind the target
    [SerializeField] private float hoverAmplitude = 0.25f; // Hover height
    [SerializeField] private float hoverSpeed = 2f;        // Hover speed
    [SerializeField] private float moveSpeed = 3f;         // Smooth follow speed

    private Transform target;
    private Vector3 initialOffset;
    private float hoverTimer;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        FindClosestTarget();
        hoverTimer = Random.Range(0f, Mathf.PI * 2f); // Randomize hover phase for variety
    }

    void Update()
    {
        if (!target)
        {
            FindClosestTarget();
            return;
        }

        hoverTimer += Time.deltaTime * hoverSpeed;

        // Desired position behind target in Z only
        Vector3 behindPosition = target.position;

        // Hover offsets
        behindPosition.x += Mathf.Cos(hoverTimer) * hoverAmplitude * 0.5f;
        behindPosition.z += followDistance * 2f; // behind in Z
        behindPosition.y = target.position.y + Mathf.Sin(hoverTimer) * hoverAmplitude;

        // Clamp Y to stay above ground
        behindPosition.y = Mathf.Max(behindPosition.y, 0.5f);

        Vector3 desiredDirection = behindPosition - transform.position;
        float desiredDistance = desiredDirection.magnitude;
        desiredDirection.Normalize();

        // Calculate the max allowed distance to move (so we don't go through walls)
        float maxDistance = desiredDistance;

        if (Physics.Raycast(transform.position, desiredDirection, out RaycastHit hit, desiredDistance, wallLayer))
        {
            // Only allow movement up to just before the wall
            maxDistance = hit.distance - 0.1f; // offset to avoid clipping
        }

        // Clamp the desired position along the direction
        behindPosition = transform.position + desiredDirection * Mathf.Max(0f, maxDistance);

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, behindPosition, Time.deltaTime * moveSpeed);

        // Always look at the target (slightly above for ominous effect)
        transform.forward = (target.position + Vector3.up * 0.5f - transform.position).normalized;
    }


    void FindClosestTarget()
    {
        FungalController[] fungals = FindObjectsOfType<FungalController>();
        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (var fungal in fungals)
        {
            float dist = Vector3.Distance(transform.position, fungal.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = fungal.transform;
            }
        }

        target = closest;
    }
}
