using UnityEngine;

public class AbilityCastIndicator : MonoBehaviour
{
    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private LineRenderer lineRenderer;

    private void OnEnable()
    {
        abilityCast.OnStart += ShowIndicator;
        abilityCast.OnUpdate += UpdateIndicator;
        abilityCast.OnComplete += HideIndicator;
    }

    private void OnDisable()
    {
        abilityCast.OnStart -= ShowIndicator;
        abilityCast.OnUpdate -= UpdateIndicator;
        abilityCast.OnComplete -= HideIndicator;
    }

    private void ShowIndicator()
    {
        lineRenderer.gameObject.SetActive(true);
    }

    private void UpdateIndicator()
    {
        var position1 = abilityCast.StartPosition;
        position1.y += 0.1f;

        var position2 = abilityCast.StartPosition + abilityCast.Direction * abilityCast.MaxDistance;
        position2.y += 0.1f;

        lineRenderer.SetPositions(new Vector3[]
        {
                position1,
                position2
        });
    }

    private void HideIndicator()
    {
        lineRenderer.gameObject.SetActive(false);
    }
}
