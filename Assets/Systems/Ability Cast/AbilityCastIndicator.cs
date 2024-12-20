using UnityEngine;

public class AbilityCastIndicator : MonoBehaviour
{
    private AbilityCast abilityCast;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        abilityCast = GetComponentInParent<AbilityCast>();
        Debug.Log($"awake {abilityCast == null}");
        lineRenderer = GetComponentInChildren<LineRenderer>(includeInactive: true);
    }

    private void OnEnable()
    {
        HideIndicator();
        Debug.Log($"enable {abilityCast == null}");

        abilityCast.OnPrepare += ShowIndicator;
        abilityCast.OnCastStart += HideIndicator;
    }

    private void OnDisable()
    {
        abilityCast.OnPrepare -= ShowIndicator;
        abilityCast.OnCastStart -= HideIndicator;
    }

    private void Update()
    {
        if (lineRenderer.gameObject.activeSelf)
        {
            UpdateIndicator();
        }
    }

    private void ShowIndicator()
    {
        UpdateIndicator();
        lineRenderer.gameObject.SetActive(true);
    }

    private void UpdateIndicator()
    {
        var position1 = abilityCast.StartPosition;
        position1.y += 0.1f;

        // todo: handle between targeted ability and directional ability
        var position2 = abilityCast.StartPosition + abilityCast.Direction * abilityCast.Data.MaxDistance;
        position2.y = position1.y;

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
