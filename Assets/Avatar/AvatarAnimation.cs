using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AvatarAnimation : MonoBehaviour
{
    private Animator animator;
    private Transform target;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public float shrinkSpeed = 2f;
    public float flashDuration = 0.2f;
    public float rotationSpeed = 360f; // Degrees per second.

    //private Material originalMaterial;
    private Coroutine possessionCoroutine;

    public void PossessFungal(FungalController fungal, UnityAction onComplete)
    {
        fungal.Controllable.Movement.Stop();
        target = fungal.transform;
        Debug.Log("Starting possession animation...");
        animator.Play("Jump");

        if (possessionCoroutine != null)
            StopCoroutine(possessionCoroutine);

        possessionCoroutine = StartCoroutine(PossessionAnimation(target, onComplete));
    }

    private IEnumerator PossessionAnimation(Transform target, UnityAction onComplete)
    {
        // Move towards target while shrinking
        while (Vector3.Distance(transform.position, target.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, shrinkSpeed * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, shrinkSpeed * Time.deltaTime);
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
            yield return null;
        }

        transform.localScale = Vector3.zero; // Ensure fully shrunk
        transform.position = target.position; // Ensure exact positioning

        // Invoke completion callback
        onComplete?.Invoke();
    }

    public void PlayReturnToBodyAnimation()
    {
        Debug.Log("Starting reverse possession animation...");

        if (possessionCoroutine != null)
            StopCoroutine(possessionCoroutine);

        // Store the original position of the avatar (target position)
        Vector3 originalPosition = transform.position;

        // Randomly offset the position within a 2-unit radius, ignoring the y-axis for horizontal movement
        var randomOffset = Random.insideUnitSphere.normalized * 2f;
        randomOffset.y = 0;

        // Add the offset to the target position to get the new position
        Vector3 randomPosition = target.position + randomOffset;

        // Start the reverse possession animation coroutine
        possessionCoroutine = StartCoroutine(ReversePossessionAnimation(randomPosition));
    }

    private IEnumerator ReversePossessionAnimation(Vector3 targetPosition)
    {
        // Duration of the animation for smoothness
        float timeElapsed = 0f;
        float moveDuration = 1f; // Adjust the duration for the desired animation speed

        Vector3 startingPosition = transform.position;
        Vector3 startingScale = transform.localScale;
        Quaternion startingRotation = transform.rotation;

        // Jump parameters
        float jumpHeight = 2f; // How high the avatar jumps
        float jumpDuration = 0.3f; // How long the jump takes
        float jumpStartTime = 0f;

        // Animate the movement, scaling, and rotation
        while (timeElapsed < moveDuration)
        {
            float lerpFactor = timeElapsed / moveDuration;

            // Add a "jump" effect by modifying the Y-position
            if (timeElapsed < jumpDuration) // During the jump phase
            {
                float jumpProgress = timeElapsed / jumpDuration;
                float jumpY = Mathf.Sin(jumpProgress * Mathf.PI) * jumpHeight; // Sinusoidal curve for smooth jump

                transform.position = new Vector3(
                    Mathf.Lerp(startingPosition.x, targetPosition.x, lerpFactor),
                    startingPosition.y + jumpY,
                    Mathf.Lerp(startingPosition.z, targetPosition.z, lerpFactor)
                );
            }
            else // After the jump, settle to target position
            {
                transform.position = Vector3.Lerp(startingPosition, targetPosition, lerpFactor);
            }

            // Smoothly grow the avatar back to its original size
            transform.localScale = Vector3.Lerp(startingScale, Vector3.one, lerpFactor);

            // Optionally, smooth the rotation
            transform.rotation = Quaternion.Slerp(startingRotation, Quaternion.LookRotation(targetPosition - transform.position), lerpFactor);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the avatar finishes exactly at the target position with the correct scale and rotation
        transform.position = targetPosition;
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.LookRotation(targetPosition - transform.position); // Optional for better rotation control
    }



}
