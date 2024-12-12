using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// this script is used to trigger events on ability cast
public class AbilityButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Button button;
    [SerializeField] private Controller controller;
    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private GameObject render;
    [SerializeField] private Image abilityImage;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private TextMeshProUGUI cooldownText;

    private Vector3 mousePosition;
    private float abilityTimer;
    private float abilityCooldown = 5f;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            abilityCast.Cast(controller.Movement.transform, controller.Movement.transform.forward, IsValidTarget);
        });
    }

    private void Update()
    {
        if (abilityTimer > 0)
        {
            SetAbilityTimer(abilityTimer - Time.deltaTime);
        }
        else
        {
            SetAbilityTimer(0);
        }
    }

    private void SetAbilityTimer(float timer)
    {
        cooldownImage.fillAmount = timer / abilityCooldown;
        abilityTimer = timer;
        button.interactable = timer == 0;
    }

    private void OnEnable()
    {
        UpdatePreview();
        abilityCast.OnShruneChanged += UpdatePreview;
        abilityCast.OnComplete += AbilityCast_OnComplete;
    }

    private void OnDisable()
    {
        abilityCast.OnShruneChanged -= UpdatePreview;
        abilityCast.OnComplete -= AbilityCast_OnComplete;
    }

    private void AbilityCast_OnComplete()
    {
        SetAbilityTimer(abilityCooldown);
    }

    private void UpdatePreview()
    {
        render.SetActive(abilityCast.Shrune);

        if (abilityCast.Shrune)
        {
            abilityImage.enabled = true;
            abilityImage.sprite = abilityCast.Shrune.Sprite;
        }
        else
        {
            abilityImage.enabled = false;
        }
    }

    bool IsValidTarget(Attackable attackable) => attackable != controller.Attackable;

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        abilityCast.StartCast(controller.Movement.transform, IsValidTarget);
        mousePosition = Input.mousePosition;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        // Get the direction from the mouse position relative to the screen space
        Vector3 mouseDirection = Input.mousePosition - mousePosition;
        mouseDirection.z = mouseDirection.y;
        mouseDirection.y = 0f; // Ignore vertical difference for XZ plane direction

        // Get the camera's forward rotation, but only around the Y axis (XZ plane)
        Quaternion cameraRotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);

        // Rotate the mouse direction by the camera's rotation
        Vector3 rotatedMouseDirection = cameraRotation * mouseDirection.normalized;

        abilityCast.UpdateCast(rotatedMouseDirection);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        //controller.SetAnimation();
        abilityCast.Cast();
    }


}
