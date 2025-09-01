using UnityEngine;
using UnityEngine.Events;

public class DJNoteController : MonoBehaviour
{
    [SerializeField] private PlantSporeEmitter plant;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float arcHeight = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float journeyLength;
    private float startTime;

    public event UnityAction OnDestinationReached;

    public void Initialize(PlantSporeEmitter targetPlant)
    {
        plant = targetPlant;
        startPos = transform.position;
        targetPos = plant.transform.position;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPos, targetPos);
    }

    private void Update()
    {
        if (!plant) return;

        // Calculate how far along the journey we are (0..1)
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        // Basic interpolation between start and target
        Vector3 newPos = Vector3.Lerp(startPos, targetPos, fracJourney);

        // Add arc height (parabola) using a sine wave
        float arc = arcHeight * Mathf.Sin(Mathf.Clamp01(fracJourney) * Mathf.PI);
        newPos.y += arc;

        transform.position = newPos;

        // When close enough, trigger effect
        if (fracJourney >= 1f)
        {
            plant.EmitSpore();
            gameObject.SetActive(false);
            OnDestinationReached?.Invoke();
        }
    }
}
