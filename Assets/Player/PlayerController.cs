using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : UnitController
{
    [Header("Player References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private Unit playerUnit;
    [SerializeField] private InventoryReference inventoryReference;
    [SerializeField] private CinemachineVirtualCamera povCamera;

    private UnitFollow unitFollow;
    private UnitDrum unitDrum;
    private UnitGlyphCollect unitGlyphCollect;

    private Material[] materials;

    public override Color Color
    {
        get => materials[0].GetColor("_Outer_Color");
        set
        {
            foreach(var material in materials)
            {
                material.SetColor("_Outer_Color", value);
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        unitFollow = GetComponent<UnitFollow>();
        unitDrum = GetComponent<UnitDrum>();
        unitGlyphCollect = GetComponent<UnitGlyphCollect>();
        unitFollow.OnDestinationReached += UnitFollow_OnDestinationReached;
        unitGlyphCollect.OnNoteHit += UnitGlyphCollect_OnNoteHit;

        // collect only the materials that are the same instance as targetMaterial
        var mats = new List<Material>();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            foreach (var mat in rend.materials)
            {
                if (mat.name.StartsWith("Player Material"))
                {
                    mats.Add(mat);
                }
            }
        }

        materials = mats.ToArray();

        playerReference.SetPlayer(this);
        playerReference.SetTargetPosition(transform.position);
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
                ApplyDefaultBehaviour();
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        playerReference.OnTargetPositionChanged += PlayerReference_OnTargetPositionChanged;
        playerReference.OnTargetInteractableChanged += PlayerReference_OnTargetInteractableChanged;
        playerReference.OnPOVCameraToggled += PlayerReference_OnPOVCameraToggled;
    }

    private void PlayerReference_OnPOVCameraToggled(bool value)
    {
        povCamera.Priority = value ? 11 : 0;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        playerReference.OnTargetPositionChanged -= PlayerReference_OnTargetPositionChanged;
        playerReference.OnTargetInteractableChanged -= PlayerReference_OnTargetInteractableChanged;
        playerReference.OnPOVCameraToggled -= PlayerReference_OnPOVCameraToggled;
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
            Destination.SetDestination(playerReference.TargetPosition);
        }
    }
}