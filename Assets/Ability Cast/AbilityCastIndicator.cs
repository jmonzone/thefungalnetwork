using UnityEngine;

public class AbilityCastIndicator : MonoBehaviour
{
    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private LineRenderer lineRenderer;

    private void OnEnable()
    {
        HideIndicator();
        abilityCast.OnStart += ShowIndicator;
        abilityCast.OnCast += HideIndicator;
    }

    private void OnDisable()
    {
        abilityCast.OnStart -= ShowIndicator;
        abilityCast.OnCast -= HideIndicator;
    }

    private void Update()
    {
        UpdateIndicator();
    }

    private void ShowIndicator()
    {
        lineRenderer.gameObject.SetActive(true);
    }

    private void UpdateIndicator()
    {
        if (!lineRenderer.gameObject.activeSelf) return;

        var position1 = abilityCast.StartPosition;
        position1.y += 0.1f;

        // todo: handle between targeted ability and directional ability
        var position2 = abilityCast.StartPosition + abilityCast.Direction * abilityCast.MaxDistance;
        if (abilityCast.Target) position2 = abilityCast.Target.position;
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
