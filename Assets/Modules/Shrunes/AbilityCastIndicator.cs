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
            var position1 = abilityCast.StartPosition;
            position1.y = 0.1f;

            var position2 = abilityCast.StartPosition + abilityCast.Direction * abilityCast.MaxDistance;
            position2.y = 0.1f;

            lineRenderer.SetPositions(new Vector3[]
            {
                position1,
                position2
            });
        };

        abilityCast.OnComplete += () => lineRenderer.gameObject.SetActive(false);
    }
}
