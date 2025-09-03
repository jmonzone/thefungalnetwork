using UnityEngine;
using UnityEngine.Events;

public class DJNoteController : MonoBehaviour
{
    [SerializeField] private INoteTarget target;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float arcHeight = 2f;
    [SerializeField] private float spiralRadius = 0.5f;
    [SerializeField] private float spiralSpeed = 4f;
    [SerializeField] private float minValue = 0.5f;
    [SerializeField] private float maxValue = 1f;

    private SpriteRenderer spriteRenderer;
    private Vector3 startPos;
    private Vector3 targetPos;
    private float journeyLength;
    private float startTime;

    public event UnityAction OnDestinationReached;

    private float spiralPhaseOffset; // unique per note
    private float spiralRadiusOffset; // optional variation in radius

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Assign random offsets when the object is created
        spiralPhaseOffset = Random.Range(0f, Mathf.PI * 2f);
        spiralRadiusOffset = Random.Range(0.8f, 1.2f); // slight radius variation
    }

    public void Initialize(INoteTarget target, Color color, float spiralPhaseOffset, float trackValue)
    {
        this.target = target;
        startPos = transform.position;
        startTime = Time.time;
        spriteRenderer.color = color;
        this.spiralPhaseOffset = spiralPhaseOffset;
        if (trackValue > 0) transform.localScale = Vector3.one * (minValue + trackValue * (maxValue - minValue));
    }

    private void Update()
    {
        if (target == null) return;


        targetPos = target.Transform.position;
        journeyLength = 2f;

        // Calculate how far along the journey we are (0..1)
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;


        // Basic interpolation between start and target
        Vector3 newPos = Vector3.Lerp(startPos, targetPos, fracJourney);

        // Add arc height (parabola) using a sine wave
        float arc = arcHeight * Mathf.Sin(Mathf.Clamp01(fracJourney) * Mathf.PI);
        newPos.y += arc;

        // Spiral offset (slight circular variation around the forward path)            // tweak for tightness
        float angle = fracJourney * spiralSpeed * Mathf.PI * 2f + spiralPhaseOffset;

        // Create an offset perpendicular to the main trajectory
        Vector3 direction = (targetPos - startPos).normalized;
        Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, direction).normalized;

        Vector3 spiralOffset = (Mathf.Cos(angle) * right + Mathf.Sin(angle) * up) * spiralRadius * spiralRadiusOffset * (1 - fracJourney);
        newPos += spiralOffset;

        transform.position = newPos;

        // When close enough, trigger effect
        if (fracJourney >= 1f)
        {
            gameObject.SetActive(false);
            OnDestinationReached?.Invoke();
        }
    }


}
