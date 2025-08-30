using System.Collections;
using UnityEngine;

public class FrogAssist : MonoBehaviour
{
    [SerializeField] private bool startAutomatically = false;
    [SerializeField] private float searchRadius = 5f;
    //[SerializeField] private float interactionInterval = 3f;
    [SerializeField] private LayerMask mushroomLayer;

    [SerializeField] private Transform tonguePivot; // Assign in Inspector
    [SerializeField] private float tongueSpeed = 10f;
    [SerializeField] private float tongueMaxScale = 5f; // Prevent absurd stretch

    private bool assistActive;
    public bool AssistActive => assistActive;

    private Coroutine assistRoutine;

    private void Start()
    {
         if (startAutomatically) StartAssistMode();
    }

    public void StartAssistMode()
    {
        //if (assistRoutine == null)
        //    assistRoutine = StartCoroutine(StartAssist());
    }

    public void StopAssistMode()
    {
        if (assistRoutine != null)
        {
            StopCoroutine(assistRoutine);
            assistRoutine = null;
        }
    }

    //private IEnumerator StartAssist()
    //{
    //    assistActive = true;

    //    while (assistActive)
    //    {
    //        yield return new WaitForSeconds(interactionInterval);

    //        InteractableMushroom target = FindNearestMushroom();

    //        if (target != null)
    //        {
    //            yield return LookAtTargetOverTime(target.transform, 0.5f);
    //            yield return ExtendTongueTowards(target.transform);
    //            target.OnBaseInteraction();
    //            yield return RetractTongue();

    //        }
    //    }
    //}

    //private InteractableMushroom FindNearestMushroom()
    //{
    //    Collider[] hits = Physics.OverlapSphere(transform.position, searchRadius, mushroomLayer);

    //    InteractableMushroom closest = null;
    //    float closestDistance = Mathf.Infinity;

    //    foreach (var hit in hits)
    //    {
    //        InteractableMushroom mushroom = hit.GetComponentInParent<InteractableMushroom>();
    //        if (mushroom != null && mushroom.IsInteractable)
    //        {
    //            float distance = Vector3.Distance(transform.position, hit.transform.position);
    //            if (distance < closestDistance)
    //            {
    //                closest = mushroom;
    //                closestDistance = distance;
    //            }
    //        }
    //    }

    //    return closest;
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }

    private IEnumerator LookAtTargetOverTime(Transform target, float duration)
    {
        var rotatable = transform.GetChild(0);
        Quaternion startRotation = rotatable.rotation;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // Keep rotation flat

        if (direction == Vector3.zero)
            yield break;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rotatable.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rotatable.rotation = targetRotation;
    }

    [SerializeField] private float scaleFactor = 0.25f;
    private IEnumerator ExtendTongueTowards(Transform target)
    {
        if (tonguePivot == null || target == null)
            yield break;

        Vector3 localScale = tonguePivot.localScale;

        // Measure world distance in horizontal plane
        Vector3 flatTarget = target.position;
        flatTarget.y = tonguePivot.transform.position.y;

        float distance = Vector3.Distance(tonguePivot.transform.position, flatTarget);
        float targetScaleZ = Mathf.Min(distance * scaleFactor, tongueMaxScale); // Clamp for max length

        float initialScaleZ = localScale.z;
        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float z = Mathf.Lerp(initialScaleZ, targetScaleZ, t);
            tonguePivot.localScale = new Vector3(localScale.x, localScale.y, z);
            elapsed += Time.deltaTime * tongueSpeed;
            yield return null;
        }

        tonguePivot.localScale = new Vector3(localScale.x, localScale.y, targetScaleZ);
    }

    private IEnumerator RetractTongue()
    {
        Vector3 localScale = tonguePivot.localScale;
        float startZ = localScale.z;
        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float z = Mathf.Lerp(startZ, 0.1f, t);
            tonguePivot.localScale = new Vector3(localScale.x, localScale.y, z);
            elapsed += Time.deltaTime * tongueSpeed;
            yield return null;
        }

        tonguePivot.localScale = new Vector3(localScale.x, localScale.y, 0.1f);
    }


}

