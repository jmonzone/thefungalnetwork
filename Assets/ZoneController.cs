using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class ZoneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private Transform zoneIndicator;

    [Header("Runtime")]
    [SerializeField] private float radius = 1f;
    [SerializeField] private Vector3 entryPosition;
    [SerializeField] private List<UnitController> units = new List<UnitController>();
    [SerializeField] private bool playerInside = false;

    public Vector3 EntryPosition => entryPosition;
    public List<UnitController> Units => units;

    public event UnityAction OnPlayerEnterZone;
    public event UnityAction OnPlayerExitZone;

    private void Update()
    {
        // Find player
        if (!playerReference.Player) return;

        // Project positions to XZ plane
        Vector3 playerPos = new Vector3(playerReference.Player.transform.position.x, 0, playerReference.Player.transform.position.z);
        Vector3 zoneCenter = new Vector3(transform.position.x, 0, transform.position.z);

        // Calculate effective radius from localScale
        radius = zoneIndicator.localScale.x * 0.5f;

        // Check distance
        float distance = Vector3.Distance(playerPos, zoneCenter);
        bool isInside = distance <= radius;

        // Trigger events on enter/exit
        if (isInside && !playerInside)
        {
            units.Add(playerReference.Player);
            playerInside = true;
            Debug.Log("hello");
            entryPosition = zoneCenter + (playerPos - zoneCenter).normalized * (radius + 1f);
            OnPlayerEnterZone?.Invoke();
        }
        else if (!isInside && playerInside)
        {
            units.Remove(playerReference.Player);
            playerInside = false;
            Debug.Log("goodbye");
            OnPlayerExitZone?.Invoke();
        }
    }

    // Optional: visualize zone in editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        float drawRadius = radius > 0 ? radius : transform.localScale.x * 0.5f;
        Gizmos.DrawWireSphere(transform.position, drawRadius);
    }
}
