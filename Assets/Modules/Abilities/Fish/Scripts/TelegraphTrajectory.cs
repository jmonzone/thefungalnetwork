using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class TelegraphTrajectory : MonoBehaviour
{
    [SerializeField] private GameObject radiusIndicator;

    public event UnityAction<Vector3, float> OnIndicatorShow;
    public event UnityAction OnIndicatorHide;

    private void Awake()
    {
        var throwFish = GetComponent<ThrowFish>();

        throwFish.OnThrowStart += targetPosition => ShowIndicator(targetPosition, throwFish.Radius);
        throwFish.OnThrowComplete += HideIndicator;
    }

    public void ShowIndicator(Vector3 targetPosition, float radius)
    {
        radiusIndicator.transform.parent = null;
        radiusIndicator.transform.position = targetPosition + Vector3.up * 0.15f;
        radiusIndicator.SetActive(true);
        radiusIndicator.transform.localScale = 2f * radius * Vector3.one;

        OnIndicatorShow?.Invoke(targetPosition, radius);
    }

    public void HideIndicator()
    {
        radiusIndicator.SetActive(false);
        OnIndicatorHide?.Invoke();
    }
}
