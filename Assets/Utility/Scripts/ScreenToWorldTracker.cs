using UnityEngine;

public class ScreenToWorldTracker : MonoBehaviour
{
    [SerializeField] private Transform worldAnchor;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        var position = mainCamera.WorldToScreenPoint(worldAnchor.position);
        transform.position = position;
    }
}
