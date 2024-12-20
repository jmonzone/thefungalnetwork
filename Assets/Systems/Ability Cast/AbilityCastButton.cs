using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// this script is used to trigger events on ability cast
public class AbilityCastButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Button button;
    [SerializeField] private Controller controller;
    [SerializeField] private GameObject render;
    [SerializeField] private Image abilityImage;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Gradient cooldownTextGradient;

    private Vector3 initialTouchPosition;
    private float abilityTimer;

    private AbilityCast AbilityCast => controller.AbilityCast;
    private float AbilityCooldown => AbilityCast.Data.Cooldown;
    private bool canCast = false;
    private bool castStarted = false;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            AbilityCast.StartCast(controller.Movement.transform.forward);
        });
    }

    private void Update()
    {
        if (!AbilityCast) return;

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
        cooldownImage.fillAmount = timer / AbilityCooldown;
        cooldownText.text = Mathf.CeilToInt(timer).ToString();
        cooldownText.color = cooldownTextGradient.Evaluate(1 - timer / AbilityCooldown);
        abilityTimer = timer;

        canCast = timer == 0;
        button.interactable = canCast;
        cooldownText.enabled = !canCast;
    }

    private void OnEnable()
    {
        UpdatePreview();
        controller.OnUpdate += UpdatePreview;
        controller.OnCastStart += AbilityCast_OnComplete;
    }

    private void OnDisable()
    {
        controller.OnUpdate -= UpdatePreview;
        controller.OnCastStart -= AbilityCast_OnComplete;
    }

    private void AbilityCast_OnComplete()
    {
        SetAbilityTimer(AbilityCooldown);
    }

    private void UpdatePreview()
    {
        if (!AbilityCast?.Data) return;

        render.SetActive(AbilityCast.Data);

        if (AbilityCast.Data)
        {
            abilityImage.enabled = true;
            abilityImage.sprite = AbilityCast.Data.AbilityIcon;
        }
        else
        {
            abilityImage.enabled = false;
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (!canCast) return;

        castStarted = true;

        // Start casting ability
        AbilityCast.Prepare();

        // Record the initial touch position
        initialTouchPosition = eventData.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!castStarted) return;

        // Calculate the direction from the initial touch position
        Vector3 dragDirection = (Vector3)eventData.position - initialTouchPosition;

        // Convert to XZ plane direction
        dragDirection.z = dragDirection.y;
        dragDirection.y = 0f; // Ignore vertical difference for XZ plane direction

        // Get the camera's forward rotation, but only around the Y axis (XZ plane)
        Quaternion cameraRotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);

        // Rotate the drag direction by the camera's rotation
        Vector3 rotatedDragDirection = cameraRotation * dragDirection.normalized;

        // Update the ability cast with the rotated direction
        AbilityCast.SetDirection(rotatedDragDirection);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (!castStarted) return;
        castStarted = false;

        // Complete the ability cast
        AbilityCast.StartCast();
    }


}
