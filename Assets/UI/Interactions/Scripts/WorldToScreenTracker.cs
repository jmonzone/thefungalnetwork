using UnityEngine;

public class WorldToScreenTracker : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        var position = mainCamera.WorldToScreenPoint(target.position);
        transform.position = position;
    }
}
