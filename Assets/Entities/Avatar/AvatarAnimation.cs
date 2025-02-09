using System.Collections;
using UnityEngine;

public class AvatarAnimation : MonoBehaviour
{
    [SerializeField] private PlayerReference controller;

    public float shrinkSpeed = 2f;
    public float flashDuration = 0.2f;
    public float rotationSpeed = 360f; // Degrees per second.

    private Animator animator;
    private Coroutine possessionCoroutine;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void StartPossessionAnimation()
    {
        Debug.Log("Starting possession animation...");
        animator.Play("Jump");

        if (possessionCoroutine != null)
            StopCoroutine(possessionCoroutine);

        possessionCoroutine = StartCoroutine(PossessionAnimation());
    }

    private IEnumerator PossessionAnimation()
    {
        yield return null;

        //var target = controller.Possessable.transform;

        //// Move towards target while shrinking
        //while (Vector3.Distance(transform.position, target.position) > 0.1f)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, target.position, shrinkSpeed * Time.deltaTime);
        //    transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, shrinkSpeed * Time.deltaTime);
        //    transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        //    yield return null;
        //}

        //transform.localScale = Vector3.zero; // Ensure fully shrunk
        //transform.position = target.position; // Ensure exact positioning

        // Invoke completion callback
        //controller.CompletePossession();
    }

    public void StartReleaseAnimation()
    {
        Debug.Log("Starting reverse possession animation...");

        if (possessionCoroutine != null) StopCoroutine(possessionCoroutine);

        //var target = controller.Possessable.transform;

        // Randomly offset the position within a 2-unit radius, ignoring the y-axis for horizontal movement
        var randomOffset = Random.insideUnitSphere.normalized * 2f;
        randomOffset.y = 0;

        // Add the offset to the target position to get the new position
        //Vector3 randomPosition = target.position + randomOffset;

        transform.localScale = Vector3.zero; // Ensure fully shrunk
        //transform.position = controller.Possessable.transform.position; // Ensure exact positioning

        // Start the reverse possession animation coroutine
        //possessionCoroutine = StartCoroutine(ReversePossessionAnimation(randomPosition));
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

        //controller.CompleteRelease();
    }
}
