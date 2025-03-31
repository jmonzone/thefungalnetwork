using UnityEngine;

public class AbilityCastIndicator : MonoBehaviour
{
    [SerializeField] private Transform targetIndicator;
    [SerializeField] private Transform rangeIndicator;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Trajectory Settings")]
    [SerializeField] private bool useTrajectory = true; // Toggle between trajectory and straight line
    [SerializeField] private int trajectoryResolution = 20; // Number of points for trajectory
    [SerializeField] private float trajectoryHeight = 3f; // Arc height for trajectory

    private void Awake()
    {
        if (!lineRenderer) lineRenderer = GetComponentInChildren<LineRenderer>(includeInactive: true);
    }

    public void ShowTargetIndicator(bool useTargetIndicator)
    {
        targetIndicator.gameObject.SetActive(useTargetIndicator);
    }

    public void ShowIndicator(bool useTrajectory)
    {
        this.useTrajectory = useTrajectory;
        lineRenderer.gameObject.SetActive(true);
        //rangeIndicator.gameObject.SetActive(true);
    }

    public void SetTargetIndicatorRadius(float radius)
    {
        targetIndicator.transform.localScale = 2f * radius * Vector3.one;
    }

    public void UpdateIndicator(Vector3 startPosition, Vector3 targetPosition, float range)
    {
        if (useTrajectory)
        {
            DrawTrajectory(startPosition, targetPosition);
        }
        else
        {
            DrawStraightLine(startPosition, targetPosition);
        }

        targetIndicator.transform.position = targetPosition;

        rangeIndicator.transform.position = startPosition;
        rangeIndicator.transform.localScale = 2f * range * Vector3.one;
    }

    private void DrawStraightLine(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;

        // Clamp magnitude to 5f
        if (direction.magnitude > 5f)
        {
            direction = direction.normalized * 5f;
            end = start + direction;
        }

        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new Vector3[] { start, end });
    }

    private void DrawTrajectory(Vector3 start, Vector3 end)
    {
        Vector3[] positions = new Vector3[trajectoryResolution];

        for (int i = 0; i < trajectoryResolution; i++)
        {
            float t = (float)i / (trajectoryResolution - 1); // Normalized time (0 to 1)
            positions[i] = CalculateParabolicPoint(start, end, t);
        }

        lineRenderer.positionCount = trajectoryResolution;
        lineRenderer.SetPositions(positions);
    }

    private Vector3 CalculateParabolicPoint(Vector3 start, Vector3 end, float t)
    {
        Vector3 position = Vector3.Lerp(start, end, t); // Interpolate in XZ plane
        position.y += trajectoryHeight * Mathf.Sin(t * Mathf.PI); // Apply arc effect

        return position;
    }

    public void HideIndicator()
    {
        targetIndicator.gameObject.SetActive(false);
        lineRenderer.gameObject.SetActive(false);
        rangeIndicator.gameObject.SetActive(false);
    }

    public void SetUseTrajectory(bool value)
    {
        useTrajectory = value;
    }
}
