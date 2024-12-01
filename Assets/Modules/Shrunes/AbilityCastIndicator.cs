using UnityEngine;

public class AbilityCastIndicator : MonoBehaviour
{
    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private LineRenderer lineRenderer;

    private void Awake()
    {
        abilityCast.OnStart += () => lineRenderer.gameObject.SetActive(true);

        abilityCast.OnUpdate += () =>
        {
            lineRenderer.SetPositions(new Vector3[]
            {
                abilityCast.StartPosition,
                abilityCast.StartPosition + abilityCast.Direction * abilityCast.MaxDistance
            });
        };

        abilityCast.OnComplete += () => lineRenderer.gameObject.SetActive(false);
    }
}
