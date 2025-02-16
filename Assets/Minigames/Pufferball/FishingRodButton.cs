using System.Collections;
using UnityEngine;

public class FishingRodButton : MonoBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private DirectionalButton directionalButton;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float range = 10f;

    private void Awake()
    {
        directionalButton.OnDragCompleted += DirectionalButton_OnDragCompleted;
    }

    private void DirectionalButton_OnDragCompleted(Vector3 direction)
    {
        direction.y = 0; // Keep it in the XZ plane
        direction.Normalize(); // Normalize to maintain consistent speed

        StopAllCoroutines();
        StartCoroutine(MoveProjectile(direction));
    }

    private IEnumerator MoveProjectile(Vector3 direction)
    {
        Vector3 startPos = playerReference.Transform.position + direction.normalized;
        Vector3 targetPos = startPos + direction * range;

        projectile.SetActive(true);

        float journey = 0f;
        float travelTime = range / speed;

        // Move forward
        while (journey < 1f)
        {
            journey += Time.deltaTime / travelTime;
            projectile.transform.position = Vector3.Lerp(startPos, targetPos, journey);
            yield return null;
        }

        journey = 0f; // Reset journey for return trip

        // Move back
        while (journey < 1f)
        {
            journey += Time.deltaTime / travelTime;
            var returnPosition = playerReference.Transform.position + (targetPos - playerReference.Transform.position).normalized;
            projectile.transform.position = Vector3.Lerp(targetPos, returnPosition, journey);
            yield return null;
        }

        projectile.SetActive(false);
    }
}
