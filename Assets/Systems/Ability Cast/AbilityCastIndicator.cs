using UnityEngine;

public class AbilityCastIndicator : MonoBehaviour
{
    [SerializeField] private Transform rangeIndicator;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>(includeInactive: true);
    }

    public void ShowIndicator()
    {
        lineRenderer.gameObject.SetActive(true);
        rangeIndicator.gameObject.SetActive(true);
    }

    public void UpdateIndicator(Vector3 startPosition, Vector3 targetPosition, float range)
    {
        var position1 = startPosition;
        position1.y += 0.1f;

        var position2 = targetPosition;
        position2.y = position1.y;

        lineRenderer.SetPositions(new Vector3[]
        {
                position1,
                position2
        });

        rangeIndicator.transform.position = startPosition;
        rangeIndicator.transform.localScale = 2f * range * Vector3.one;
    }

    public void HideIndicator()
    {
        lineRenderer.gameObject.SetActive(false);
        rangeIndicator.gameObject.SetActive(false);
    }
}
