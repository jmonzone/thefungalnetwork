using UnityEngine;
using System.Collections;

public class PlantSporeEmitter : MonoBehaviour
{
    [Header("Plant Settings")]
    [SerializeField] private Transform plantModel; // The visual mesh/child that will bounce
    [SerializeField] private float bounceScale = 1.2f;
    [SerializeField] private float bounceDuration = 0.3f;

    [Header("Spore Settings")]
    [SerializeField] private GameObject sporePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float initialUpForce = 2f;
    [SerializeField] private float driftStrength = 1f;
    [SerializeField] private float fallSpeed = 1f;

    [Header("Timing")]
    [SerializeField] private float emitInterval = 3f;

    private void Start()
    {
        StartCoroutine(EmitRoutine());
    }

    private IEnumerator EmitRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(emitInterval);

            // Bounce animation
            yield return StartCoroutine(BouncePlant());

            // Spawn spore
            if (sporePrefab && spawnPoint)
            {
                GameObject spore = Instantiate(sporePrefab, spawnPoint.position, Quaternion.identity);
                StartCoroutine(SporeMotion(spore));
            }
        }
    }

    private IEnumerator BouncePlant()
    {
        Vector3 startScale = plantModel.localScale;
        Vector3 targetScale = startScale * bounceScale;
        float t = 0f;

        // Scale up
        while (t < bounceDuration)
        {
            t += Time.deltaTime;
            float progress = t / bounceDuration;
            plantModel.localScale = Vector3.Lerp(startScale, targetScale, Mathf.Sin(progress * Mathf.PI));
            yield return null;
        }

        plantModel.localScale = startScale;
    }

    private IEnumerator SporeMotion(GameObject spore)
    {
        Rigidbody rb = spore.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = spore.AddComponent<Rigidbody>();
            rb.useGravity = false; // We’ll simulate feather-like fall
        }

        float upwardVelocity = initialUpForce;
        float y = spore.transform.position.y;

        while (true)
        {
            y += upwardVelocity * Time.deltaTime;
            upwardVelocity = Mathf.Lerp(upwardVelocity, -fallSpeed, Time.deltaTime * 0.5f); // smoothly switch to falling

            float drift = Mathf.Sin(Time.time * 2f) * driftStrength; // side-to-side motion

            spore.transform.position += new Vector3(drift * Time.deltaTime, upwardVelocity * Time.deltaTime, 0f);

            // stop when it touches the ground (y <= 0 assumed ground)
            if (spore.transform.position.y <= 0f)
            {
                spore.transform.position = new Vector3(spore.transform.position.x, 0f, spore.transform.position.z);
                yield break;
            }

            yield return null;
        }
    }
}
