using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    [SerializeField] private GameReference playerReference;
    [SerializeField] private DirectionalButton directionalButton;
    [SerializeField] private AbilityCastIndicator abilityCastIndicator;
    [SerializeField] private CooldownHandler cooldownHandler;

    [Header("UI")]
    [SerializeField] private Image abilityIcon;
    [SerializeField] private Image abilityBackground;
    [SerializeField] private TextMeshProUGUI abilityText;

    private Ability ability;
    private bool isDown = false;
    private Vector3 targetPosition;

    // Initialization
    private void Awake()
    {
        // Subscribing to directional button events
        directionalButton.OnPointerUp += DirectionalButton_OnPointerUp;
        directionalButton.OnPointerDown += DirectionalButton_OnPointerDown;
        directionalButton.OnDragUpdated += OnDragUpdated;
        directionalButton.OnDragCompleted += OnDragCompleted;

        // If the ability is assigned, subscribe to its events
        if (ability != null)
        {
            ability.OnCancel += OnDragCanceled;
            ability.OnAvailabilityChanged += UpdateAbility;
        }
    }

    private void Start()
    {
        // Initialize cooldown handler if ability is assigned
        if (ability != null)
        {
            cooldownHandler.AssignCooldownModel(ability);
        }
    }

    // Event Subscription/Unsubscription
    private void OnEnable()
    {
        if (playerReference.ClientPlayer?.Fungal == null || ability == null) return;

        playerReference.ClientPlayer.Fungal.OnDeath += Fungal_OnDeath;
        playerReference.ClientPlayer.Fungal.OnRespawnComplete += UpdateAbility;
    }

    private void OnDisable()
    {
        if (playerReference.ClientPlayer?.Fungal == null || ability == null) return;

        playerReference.ClientPlayer.Fungal.OnDeath -= Fungal_OnDeath;
        playerReference.ClientPlayer.Fungal.OnRespawnComplete -= UpdateAbility;
    }

    // Ability Management
    public void AssignAbility(Ability newAbility)
    {
        // Unsubscribe from old ability events if any
        if (ability != null)
        {
            ability.OnCancel -= OnDragCanceled;
            ability.OnAvailabilityChanged -= UpdateAbility;
        }

        ability = newAbility;

        abilityText.text = ability.Id;
        abilityBackground.color = ability.BackgroundColor;
        abilityIcon.sprite = ability.Image;

        // Subscribe to new ability events
        if (ability != null)
        {
            ability.OnCancel += OnDragCanceled;
            ability.OnAvailabilityChanged += UpdateAbility;
            cooldownHandler.AssignCooldownModel(ability);
            UpdateAbility(); // Update ability state
        }
    }

    private void UpdateAbility()
    {
        if (ability != null)
        {
            directionalButton.enabled = ability.IsAvailable;
        }
    }

    // Input Handling
    private void DirectionalButton_OnPointerDown()
    {
        if (ability == null || ability.IsOnCooldown) return;

        isDown = true;
        ability.PrepareAbility();

        if (ability is DirectionalAbility directionalAbility)
        {
            abilityCastIndicator.ShowIndicator(directionalAbility.UseTrajectory);
            abilityCastIndicator.ShowTargetIndicator(directionalAbility.UseTrajectory);
        }
    }

    private void DirectionalButton_OnPointerUp()
    {
        if (ability == null) return;

        if (ability is DirectionalAbility directionalAbility)
        {
            CastDirectionalAbility(directionalAbility, directionalAbility.DefaultTargetPosition);
        }
        else
        {
            CastSelfTargetAbility();
        }
    }

    private void CastSelfTargetAbility()
    {
        if (ability == null) return;

        isDown = false;
        ability.CastAbility();
    }

    private void CastDirectionalAbility(DirectionalAbility directionalAbility, Vector3 targetPosition)
    {
        if (ability == null) return;

        isDown = false;
        directionalAbility.CastAbility(targetPosition);
        abilityCastIndicator.HideIndicator();
    }

    // Drag & Target Handling
    private void OnDragUpdated(Vector3 direction)
    {
        if (ability == null) return;

        if (ability is DirectionalAbility directionalAbility)
        {
            var clampedDirection = Vector3.ClampMagnitude(direction, directionalAbility.Range);
            var startPosition = playerReference.ClientPlayer.Fungal.transform.position;
            targetPosition = startPosition + clampedDirection;
            targetPosition.y = 0; // Keep in the XZ plane      
        }
    }

    private void OnDragCompleted(Vector3 direction)
    {
        if (ability == null || ability.IsOnCooldown) return;

        if (ability is DirectionalAbility directionalAbility)
        {
            CastDirectionalAbility(directionalAbility, directionalAbility.DefaultTargetPosition);
        }
        else
        {
            CastSelfTargetAbility();
        }
    }

    private void OnDragCanceled()
    {
        abilityCastIndicator.HideIndicator();
    }

    // UI & Ability State Update
    private void LateUpdate()
    {
        if (isDown && ability != null)
        {
            ability.ChargeAbility();


            if (ability is DirectionalAbility directionalAbility)
            {
                // Use the target position either from the drag or default
                var targetPos = directionalButton.DragStarted ? targetPosition : directionalAbility.DefaultTargetPosition;
                abilityCastIndicator.UpdateIndicator(playerReference.ClientPlayer.Fungal.transform.position, targetPos, directionalAbility.Range);
                abilityCastIndicator.SetTargetIndicatorRadius(ability.Radius);
            }
        }
    }

    // Fungal Death Handling
    private void Fungal_OnDeath(bool killed)
    {
        abilityCastIndicator.HideIndicator();
        directionalButton.enabled = false;
    }
}
