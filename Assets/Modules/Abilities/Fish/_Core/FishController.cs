using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishController : MonoBehaviour, IAbilityHolder
{
    [Header("Customization")]
    [SerializeField] private float swimSpeed = 1f;
    [SerializeField] private float score;
    [SerializeField] private bool isPickedUp = false;
    [SerializeField] private FungalThrow fishAbility;
    [SerializeField] private float respawnTime = 1f;

    [Header("Audio")]
    [SerializeField] private float audioPitch;
    [SerializeField] private List<AudioClip> audioClips;

    public Movement Movement { get; private set; }
    public ThrowFish ThrowFish { get; private set; }
    public FungalController Fungal { get; private set; }
    public FungalThrow Ability => fishAbility;

    public bool IsPickedUp => isPickedUp;
    public float Score => score;

    bool IAbilityHolder.CanBePickedUp(FungalController fungal)
    {
        return !isPickedUp;
    }

    Vector3 IAbilityHolder.Position => transform.position;

    private AudioSource audioSource;
    private Vector3 spawnPosition;

    public event UnityAction OnPickedUp;
    public event UnityAction OnRespawnComplete;
    public event UnityAction OnPrepareThrow;

    private void Awake()
    {
        Movement = GetComponent<Movement>();
        ThrowFish = GetComponent<ThrowFish>();
        audioSource = GetComponent<AudioSource>();
        SetSpawnPosition(transform.position);
    }

    private void Start()
    {
        OnRespawnComplete += HandleRespawn;
    }

    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position;
    }

    //todo: centralize with Fungal Controller
    public void Respawn()
    {
        //Debug.Log($"ReturnToRadialMovement {name}");
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return Movement.ScaleOverTime(0.1f, 0f);
        yield return new WaitForSeconds(respawnTime);
        transform.position = spawnPosition;
        Movement.SetSpeed(swimSpeed);
        Movement.StartRadialMovement(spawnPosition, true);
        yield return Movement.ScaleOverTime(0.5f, 1f);

        OnRespawnComplete?.Invoke();
    }

    public void HandleRespawn()
    {
        Fungal = null;
        isPickedUp = false;
    }

    public void PrepareThrow()
    {
        OnPrepareThrow?.Invoke();
    }

    public void PickUp(FungalController fungal)
    {
        //Debug.Log("PickUp");
        HandlePickup(fungal);
        OnPickedUp?.Invoke();
    }

    public void HandlePickup(FungalController fungal)
    {
        Fungal = fungal;
        Fungal.AssignAbility(AbilitySlot.EXTERNAL, fishAbility);

        isPickedUp = true;

        audioSource.clip = audioClips.GetRandomItem();
        audioSource.pitch = audioPitch;
        audioSource.Play();

        Movement.SetSpeed(10);
        Movement.Follow(fungal.transform);
    }
}