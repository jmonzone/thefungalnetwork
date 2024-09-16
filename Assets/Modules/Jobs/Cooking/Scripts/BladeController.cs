using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeController : MonoBehaviour
{
    private Camera mainCamera;
    private Collider bladeCollider;
    private TrailRenderer trail;
    private bool isSlicing;

    public Vector3 Direciton { get; private set; }
    public float sliceForce;
    public float minVelocity = 0.01f;

    private void Awake()
    {
        mainCamera = Camera.main;
        bladeCollider = GetComponent<Collider>();
        trail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnEnable()
    {
        StopSlicing();
    }

    private void OnDisable()
    {
        StopSlicing();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartSlicing();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopSlicing();
        }
        else if (isSlicing)
        {
            ContinueSlicing();
        }
    }

    private void StartSlicing()
    {
        Vector3 newPosition = GetWorldPositionOnPlane(Input.mousePosition, transform.position.z);

        transform.position = newPosition;

        isSlicing = true;
        bladeCollider.enabled = true;
        trail.enabled = true;
        trail.Clear();
    }

    private void StopSlicing()
    {
        isSlicing = false;
        bladeCollider.enabled = false;
        trail.enabled = false;
    }

    private void ContinueSlicing()
    {
        Vector3 newPosition = GetWorldPositionOnPlane(Input.mousePosition, transform.position.z);

        Direciton = newPosition - transform.position;

        float velocity = Direciton.magnitude / Time.deltaTime;
        bladeCollider.enabled = velocity > minVelocity;

        transform.position = newPosition;
    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }
}
