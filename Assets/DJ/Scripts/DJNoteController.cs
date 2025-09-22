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
    private float initialScale;

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

    public void Initialize(INoteTarget target, DJTrack track, float spiralPhaseOffset, float trackValue)
    {
        this.target = target;
        startPos = transform.position;
        startTime = Time.time;
        spriteRenderer.sprite = track.Glyph.Sprite;

        this.spiralPhaseOffset = spiralPhaseOffset;
        initialScale = minValue + trackValue * (maxValue - minValue);
        if (trackValue > 0) transform.localScale = Vector3.one * initialScale;
    }

    private void Update()
    {
        if (target == null) return;

        targetPos = target.Transform.position;
        journeyLength = 2f;

        // Calculate fraction of journey (0..1)
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = Mathf.Clamp01(distCovered / journeyLength);

        // --- Position ---
        Vector3 newPos = Vector3.Lerp(startPos, targetPos, fracJourney);

        // Arc for nice jump / trajectory
        float arc = arcHeight * Mathf.Sin(fracJourney * Mathf.PI);
        newPos.y += arc;

        // Spiral offset
        float angle = fracJourney * spiralSpeed * Mathf.PI * 2f + spiralPhaseOffset;
        Vector3 direction = (targetPos - startPos).normalized;
        Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, direction).normalized;
        Vector3 spiralOffset = (Mathf.Cos(angle) * right + Mathf.Sin(angle) * up) * spiralRadius * spiralRadiusOffset * (1 - fracJourney);
        newPos += spiralOffset;

        transform.position = newPos;

        // --- Scale down smoothly as it reaches target ---
        float scale = Mathf.Lerp(initialScale, 0.3f, fracJourney); // 0.3f matches the collect animation start
        transform.localScale = Vector3.one * scale;

        // --- Trigger collection ---
        if (fracJourney >= .9f)
        {
            gameObject.SetActive(false);

            // Optional: trigger the glyph collect reveal on the player
            if (OnDestinationReached != null)
                OnDestinationReached.Invoke();
        }
    }
}
