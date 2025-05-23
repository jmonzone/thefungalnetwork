using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AbilitySlot
{
    INTERNAL,
    EXTERNAL
}

public class FungalController : MonoBehaviour
{
    [SerializeField] private FungalData data;
    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float respawnDuration = 5f;
    [SerializeField] private GameObject shieldRenderer;
    [SerializeField] private GameObject trailRenderers;
    [SerializeField] private GameObject stunAnimation;
    [SerializeField] private Material outlineMaterialPrefab;

    [Header("Debug")]
    [SerializeField] private Ability externalAbility;
    [SerializeField] private Ability innateAbility;

    private Renderer modelRenderer;
    private Material outlineMaterial;

    public FungalData Data => data;
    public float BaseSpeed => 3f;

    public ulong Id { get; set; }
    public bool IsBot { get; set; }
    public Vector3 SpawnPosition { get; set; }
    public float RemainingRespawnTime { get; private set; }
    public bool CanRespawn { get; set; } = true;

    public bool IsDead { get; private set; }
    public Ability ExternalAbility => externalAbility;
    public Ability InnateAbility => innateAbility;

    public Movement Movement { get; private set; }
    public Health Health { get; private set; }
    public MovementAnimations Animations { get; private set; }
    public MaterialFlasher MaterialFlasher { get; private set; }

    public event UnityAction OnSpeedModified;
    public event UnityAction<bool> OnShieldToggled;
    public event UnityAction<bool> OnTrailToggled;
    public event UnityAction<bool> OnDeath;

    public event UnityAction OnRespawnStart;
    public event UnityAction OnRespawnComplete;

    public event UnityAction OnAbilityChanged;

    public UnityAction<float, bool> HandleSpeedModifier;
    public UnityAction<bool> HandleSpeedReset;

    public UnityAction<bool> HandleDeath;
    public UnityAction HandleRespawn;

    public UnityAction<GameObject, Vector3> HandleSpawnObject;
    public UnityAction<GameObject> OnObjectHasSpawned;

    private void Awake()
    {
        Movement = GetComponent<Movement>();
        Movement.SetSpeed(baseSpeed);

        Health = GetComponent<Health>();

        HandleSpeedModifier = ApplySpeedModifier;
        HandleSpeedReset = ApplySpeedReset;

        HandleDeath = ApplyDeath;
        HandleRespawn = ApplyRespawn;
        HandleSpawnObject = SpawnObject;

        if (data) InitializePrefab(data);
    }

    private void Start()
    {
        SpawnPosition = transform.position;

        Health.OnDamaged += Health_OnDamaged;
    }

    private void Health_OnDamaged(DamageEventArgs args)
    {
        //Debug.Log($"Health_OnDamaged {args.lethal}");
        if (args.lethal)
        {
            Die(args.SelfInflicted);
        }
        else
        {
            Animations.PlayHitAnimation();
            MaterialFlasher.FlashColor(Color.red);
        }
    }

    public void InitializePrefab(FungalData fungal)
    {
        //Debug.Log($"InitializePrefab {name} {fungal}");
        data = fungal;

        var model = Instantiate(data.Prefab, transform);
        modelRenderer = model.GetComponentInChildren<Renderer>();

        // Get existing materials and make a new array with one extra slot
        var originalMats = modelRenderer.sharedMaterials;
        Material[] newMats = new Material[originalMats.Length + 1];

        // Create a runtime copy of the outline material
        outlineMaterial = new Material(outlineMaterialPrefab);
        outlineMaterial.SetFloat("_OutlineThickness", 0f); // Set thickness to 0

        // Assign the outline material first (rendered behind others)
        newMats[0] = originalMats[0];
        newMats[1] = outlineMaterial;
        modelRenderer.materials = newMats;

        Animations = model.AddComponent<MovementAnimations>();
        MaterialFlasher = model.AddComponent<MaterialFlasher>();
        MaterialFlasher.flashDuration = 0.5f;

        AssignAbility(AbilitySlot.INTERNAL, Data.Ability1);
        AssignAbility(AbilitySlot.EXTERNAL, Data.Ability2);
    }

    private IEnumerator RespawnRoutine()
    {
        OnRespawnStart?.Invoke();
        RemainingRespawnTime = respawnDuration;

        while (RemainingRespawnTime > 0f)
        {
            yield return null; // Wait for next frame
            RemainingRespawnTime -= Time.deltaTime;
        }

        RemainingRespawnTime = 0f;

        HandleRespawn?.Invoke();
    }

    private void ApplyRespawn()
    {
        Health.ReplenishHealth();
        transform.position = SpawnPosition;

        ApplyRespawnEffects();
    }

    public void ApplyRespawnEffects()
    {
        IsDead = false;
        Movement.enabled = true;

        Animations.PlaySpawnAnimation();
        MaterialFlasher.FlashColor(Color.white);
        OnRespawnComplete?.Invoke();
    }

    public void Die(bool selfDestruct)
    {
        HandleDeath?.Invoke(selfDestruct);
    }

    public void ApplyDeath(bool selfDestruct)
    {
        IsDead = true;
        Movement.enabled = false;

        AssignAbility(AbilitySlot.EXTERNAL, null);

        Animations.PlayDeathAnimation();
        MaterialFlasher.FlashColor(Color.red);

        Movement.Stop();

        OnDeath?.Invoke(selfDestruct);

        if (CanRespawn) StartCoroutine(RespawnRoutine());
    }

    public void ToggleShieldRenderers(bool value)
    {
        shieldRenderer.SetActive(value);
        OnShieldToggled?.Invoke(value);
    }

    public void ToggleTrailRenderers(bool value)
    {
        trailRenderers.SetActive(value);
        OnTrailToggled?.Invoke(value);
    }


    public void ModifySpeed(float modifer, float duration, bool showStunAnimation)
    {
        StartCoroutine(ResetSpeed(duration, showStunAnimation));
        HandleSpeedModifier?.Invoke(modifer, showStunAnimation);
    }

    public void ApplySpeedModifier(float modifer, bool showStunAnimation)
    {
        if (showStunAnimation) stunAnimation.SetActive(true);
        Movement.SetSpeedModifier(modifer);
    }

    private IEnumerator ResetSpeed(float duration, bool showStunAnimation)
    {
        yield return new WaitForSeconds(duration);
        HandleSpeedReset?.Invoke(showStunAnimation);
    }

    public void ApplySpeedReset(bool showStunAnimation)
    {
        if (showStunAnimation) stunAnimation.SetActive(false);
        Movement.ResetSpeedModifier();
    }

    public bool CanApplyAbility(Ability abilityToApply)
    {
        if (IsDead) return false;
        else if (externalAbility)
        {
            if (externalAbility is FungalThrow) return false;
            else if (externalAbility.Id == abilityToApply.Id) return false;
            else return true;
        }
        else return true;
    }

    private readonly List<Ability> cachedAbilities = new();

    public void AssignAbility(AbilitySlot slot, Ability ability)
    {
        ref Ability target = ref GetAbilityByIndex(slot);

        if (!ability)
        {
            target = null;
            SetOutline(slot, null);
        }
        else
        {
            Ability assigned = cachedAbilities.Find(a => a.Id == ability.Id);

            if (!assigned)
            {
                assigned = Instantiate(ability);
                assigned.Initialize(this, slot);
                cachedAbilities.Add(assigned);
            }
            else
            {
                assigned.OnReassigned(slot);
            }

            target = assigned;
            SetOutline(slot, ability);
        }

        OnAbilityChanged?.Invoke();
    }

    public void RemoveAbility(AbilitySlot slot)
    {
        AssignAbility(slot, slot == AbilitySlot.EXTERNAL ? data.Ability2 : null);
    }

    private void SetOutline(AbilitySlot slot, Ability ability)
    {
        if (slot != AbilitySlot.EXTERNAL) return;

        if (ability == null || data.Ability2?.Id == ability.Id)
        {
            outlineMaterial.SetFloat("_OutlineThickness", 0f);
        }
        else
        {
            outlineMaterial.SetFloat("_OutlineThickness", 0.002f);
            outlineMaterial.SetColor("_OutlineColor", ability.BackgroundColor);
        }
    }

    // Helper function to get reference to the ability field by index
    private ref Ability GetAbilityByIndex(AbilitySlot slot)
    {
        switch (slot)
        {
            case AbilitySlot.EXTERNAL: return ref externalAbility;
            case AbilitySlot.INTERNAL: return ref innateAbility;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(slot), "Invalid ability index. Must be 1 or 2.");
        }
    }

    public void RequestSpawnObject(GameObject prefab, Vector3 spawnPosition)
    {
        //Debug.Log("RequestSpawnObject");

        HandleSpawnObject?.Invoke(prefab, spawnPosition);
    }

    private void SpawnObject(GameObject prefab, Vector3 spawnPosition)
    {
        //Debug.Log("SpawnObject");

        var projectile = Instantiate(prefab, spawnPosition, Quaternion.identity);
        OnObjectSpawned(projectile);
    }

    public void OnObjectSpawned(GameObject obj)
    {
        //Debug.Log("OnObjectSpawned");
        OnObjectHasSpawned?.Invoke(obj);
    }
}
