using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;

public class CameraRotationController : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public CinemachineVirtualCamera vCam; // child vcam

    [Header("Follow")]
    public Vector3 followOffset = Vector3.zero;
    public float followSmoothTime = 0.12f;

    [Header("Orbit")]
    public float orbitSensitivity = 0.2f;      // degrees per pixel
    public float orbitSmoothTime = 0.06f;
    public float minPitch = 15f;               // near horizon (when close)
    public float maxPitch = 60f;               // top-down (when far)
    public float minPitchClamp = -30f;         // absolute clamp lower bound for manual pitch
    public float maxPitchClamp = 80f;          // absolute clamp upper bound for manual pitch

    [Header("Zoom / Dolly")]
    public float minDistance = 2f;
    public float maxDistance = 12f;
    public float zoomSpeed = 5f;               // used for scroll and pinch
    public float zoomSmoothTime = 0.12f;

    [Header("FOV")]
    public float fovMin = 30f;                 // FOV when close
    public float fovMax = 60f;                 // FOV when far
    public float fovSmoothTime = 0.12f;

    [Header("Pokémon GO Effect")]
    [Range(0f, 1f)]
    [Tooltip("Blend between manual pitch and auto pitch determined by zoom/dolly. 0 = only manual pitch, 1 = only auto (zoom driven)")]
    public float pogoEffectAmount = 0.7f;

    // Internal state
    float targetYaw;
    float yaw;
    float yawVel;

    float targetPitch;   // manual pitch (changed by 2-finger drag)
    float pitch;
    float pitchVel;

    float targetDistance;
    float distance;
    float distanceVel;

    float targetHeight;
    float height;
    float heightVel;

    float targetFov;
    float currentFov;
    float fovVel;

    Vector3 followVelocity;
    Vector2 lastMousePos;
    bool dragging;

    void Start()
    {
        if (vCam == null) Debug.LogError("CameraRotationController: vCam not assigned!");

        // Init distance from child vCam local z (assumes camera sits at local z = -distance)
        if (vCam != null)
        {
            float z = vCam.transform.localPosition.z;
            distance = targetDistance = Mathf.Abs(z) > 0.001f ? -z : (minDistance + maxDistance) * 0.5f;
        }
        else
        {
            distance = targetDistance = (minDistance + maxDistance) * 0.5f;
        }

        // init yaw/pitch from current rig rotation
        Vector3 e = transform.eulerAngles;
        yaw = targetYaw = e.y;
        float normalizedX = (e.x > 180f) ? (e.x - 360f) : e.x;
        pitch = targetPitch = Mathf.Clamp(normalizedX, minPitchClamp, maxPitchClamp);

        // init fov
        if (vCam != null)
        {
            currentFov = vCam.m_Lens.FieldOfView;
            targetFov = Mathf.Lerp(fovMin, fovMax, Mathf.InverseLerp(minDistance, maxDistance, distance));
        }
    }

    void Update()
    {
        if (target == null || vCam == null) return;
        ApplyFollowRotationZoom();

        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

        HandleOrbitInput();    // one-finger = yaw, two-finger = yaw+pitch
        HandleZoomInput();     // scroll/pinch -> ZoomCamera(..) per provided snippet

    }

    void HandleOrbitInput()
    {
        // One finger touch -> yaw
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved)
            {
                targetYaw += t.deltaPosition.x * orbitSensitivity * (targetDistance < 0 ? -1 : 1);
                //manualTargetPitch += t.deltaPosition.y * orbitSensitivity * 0.5f;
            }
        }
        else if (Application.isEditor)
        {
            // Mouse drag (Editor) -> yaw
            if (Input.GetMouseButtonDown(0))
            {
                dragging = true;
                lastMousePos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
            }

            if (dragging)
            {
                Vector2 current = Input.mousePosition;
                Vector2 delta = current - lastMousePos;
                lastMousePos = current;

                targetYaw += delta.x * orbitSensitivity * (targetDistance < 0 ? -1 : 1);
                //manualTargetPitch += delta.y * orbitSensitivity;
            }
        }
    }

    void HandleZoomInput()
    {
        // Mouse scroll / trackpad
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            ZoomCamera(scroll * zoomSpeed);
        }

        // Pinch zoom
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prevT0 = t0.position - t0.deltaPosition;
            Vector2 prevT1 = t1.position - t1.deltaPosition;

            float prevDist = (prevT0 - prevT1).magnitude;
            float currDist = (t0.position - t1.position).magnitude;

            float delta = currDist - prevDist;

            ZoomCamera(delta * zoomSpeed * 0.1f * Time.deltaTime);
        }
    }

    // ZoomCamera adjusts the target distance (dolly). We'll also update targetFov.
    void ZoomCamera(float delta)
    {
        targetDistance -= delta;
        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);

        targetHeight -= delta;
        targetHeight = Mathf.Clamp(targetHeight, 0, 8);

        // update targetFov mapped to distance (close -> fovMin, far -> fovMax)
        float t = Mathf.InverseLerp(minDistance, maxDistance, targetDistance);
        targetFov = Mathf.Lerp(fovMin, fovMax, t);
    }

    void ApplyFollowRotationZoom()
    {
        // Smooth follow the target position + offset
        Vector3 desiredPos = target.position + followOffset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref followVelocity, followSmoothTime);

        // Smooth yaw and pitch (manual)
        yaw = Mathf.SmoothDampAngle(yaw, targetYaw, ref yawVel, orbitSmoothTime);

        //manualTargetPitch = Mathf.Clamp(manualTargetPitch, -10, 10);
        //manualPitch = Mathf.SmoothDampAngle(manualPitch, manualTargetPitch, ref manualPitchVel, orbitSmoothTime);
        //vCam.transform.localRotation = Quaternion.Euler(manualPitch, vCam.transform.localRotation.eulerAngles.y, 0f);

        // Auto-pitch derived from zoom/dolly: close -> near horizon (minPitch), far -> top-down (maxPitch)
        float zoomT = Mathf.InverseLerp(minDistance, maxDistance, distance); // note: uses current distance
        float autoPitch = Mathf.Lerp(minPitch, maxPitch, zoomT);

        // Blend manual targetPitch and autoPitch by pogoEffectAmount
        float blendedTargetPitch = Mathf.Lerp(targetPitch, autoPitch, pogoEffectAmount);

        // Smooth pitch toward blended target
        pitch = Mathf.SmoothDampAngle(pitch, blendedTargetPitch, ref pitchVel, orbitSmoothTime);

        // Apply rotation to rig
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Smoothly approach the target distance
        distance = Mathf.SmoothDamp(distance, targetDistance, ref distanceVel, zoomSmoothTime);
        height = Mathf.SmoothDamp(height, targetHeight, ref heightVel, zoomSmoothTime);

        // Apply dolly by moving the child vCam along local Z (assumes vCam initial localPos.z is -distance)
        Vector3 camLocal = vCam.transform.localPosition;
        //camLocal.y = height;
        camLocal.z = -distance;

        // Smoothly approach the target distance
        vCam.transform.localPosition = camLocal;

        // Smoothly update FOV
        currentFov = Mathf.SmoothDamp(currentFov, targetFov, ref fovVel, fovSmoothTime);
        vCam.m_Lens.FieldOfView = currentFov;
    }
}
