using UnityEngine;
using UnityEngine.Events;

public class TelegraphTrajectory : MonoBehaviour
{
    [SerializeField] private GameObject radiusIndicator;

    public void ShowIndicator(Vector3 targetPosition, float radius)
    {
        radiusIndicator.transform.parent = null;
        radiusIndicator.transform.position = targetPosition + Vector3.up * 0.15f;
        radiusIndicator.SetActive(true);
        radiusIndicator.transform.localScale = 2f * radius * Vector3.one;
    }

    public void HideIndicator()
    {
        radiusIndicator.SetActive(false);
    }
}
