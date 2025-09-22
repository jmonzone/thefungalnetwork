using UnityEngine;

public class PlayerController : UnitController
{
    [Header("Player References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private Unit playerUnit;
    [SerializeField] private InventoryReference inventoryReference;

    private UnitFollow unitFollow;
    private UnitDestination unitDestination;
    private UnitDrum unitDrum;
    private UnitGlyphCollect unitGlyphCollect;

    protected override void Awake()
    {
        base.Awake();

        unitFollow = GetComponent<UnitFollow>();
        unitDestination = GetComponent<UnitDestination>();
        unitDrum = GetComponent<UnitDrum>();
        unitGlyphCollect = GetComponent<UnitGlyphCollect>();

        unitFollow.OnDestinationReached += UnitFollow_OnDestinationReached;
        unitGlyphCollect.OnNoteHit += UnitGlyphCollect_OnNoteHit;

        playerReference.SetPlayer(this);
        playerReference.SetTargetPosition(transform.position);

        var instance = ScriptableObject.CreateInstance<UnitInstance>();
        instance.Initialize(playerUnit);

        Initialize(instance);
    }

    private void UnitGlyphCollect_OnNoteHit(DJTrack track)
    {
        inventoryReference.IncreaseShrune(track.Glyph);
    }

    private void UnitFollow_OnDestinationReached()
    {
        switch (playerReference.TargetInteractable)
        {
            case PlantSporeEmitter plant:
                unitDrum.SetPlant(plant);
                ApplyBehaviour(unitDrum);
                break;
            default:
                playerReference.TargetInteractable.Select();
                SetDefaultBehaviour();
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        playerReference.OnTargetPositionChanged += PlayerReference_OnTargetPositionChanged;
        playerReference.OnTargetInteractableChanged += PlayerReference_OnTargetInteractableChanged; ;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        playerReference.OnTargetPositionChanged -= PlayerReference_OnTargetPositionChanged;
        playerReference.OnTargetInteractableChanged -= PlayerReference_OnTargetInteractableChanged;
    }

    private void PlayerReference_OnTargetInteractableChanged()
    {
        if (playerReference.TargetInteractable != null)
        {
            unitFollow.SetTarget(playerReference.TargetInteractable.Transform);
            ApplyBehaviour(unitFollow);
        }
    }

    private void PlayerReference_OnTargetPositionChanged()
    {
        if (playerReference.TargetInteractable == null)
        {
            SetLookPosition(playerReference.TargetPosition);
            unitDestination.SetDestination(playerReference.TargetPosition);
            ApplyBehaviour(unitDestination);
        }
    }
}