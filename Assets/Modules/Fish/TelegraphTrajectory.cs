using Unity.Netcode;
using UnityEngine;

public class TelegraphTrajectory : MonoBehaviour
{
    [SerializeField] private GameObject radiusIndicator;

    private void Awake()
    {
        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowStart += targetPosition => OnThrowStartClientRpc(targetPosition, throwFish.Radius);
        throwFish.OnThrowComplete += HideIndicator;
    }

    private void OnThrowStartClientRpc(Vector3 targetPosition, float radius)
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
