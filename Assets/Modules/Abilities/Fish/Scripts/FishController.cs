using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishController : MonoBehaviour
{
    [Header("Customization")]
    [SerializeField] private Ability ability;
    [SerializeField] private float swimSpeed = 1f;
    [SerializeField] private float score;
    [SerializeField] private bool useTrajectory = false;
    [SerializeField] private bool isPickedUp = false;

    [Header("Audio")]
    [SerializeField] private float audioPitch;
    [SerializeField] private List<AudioClip> audioClips;

    public Movement Movement { get; private set; }
    public ThrowFish ThrowFish { get; private set; }
    public FungalController Fungal { get; private set; }

    public bool IsPickedUp => isPickedUp;
    public float Score => score;
    public bool UseTrajectory => useTrajectory;

    public Ability Ability => ability;

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

        spawnPosition = transform.position;
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
        yield return new WaitForSeconds(2f);
        transform.position = spawnPosition;
        Movement.SetSpeed(swimSpeed);
        Movement.StartRadialMovement(spawnPosition, true);
        yield return Movement.ScaleOverTime(0.5f, 1f);

        OnRespawnComplete?.Invoke();

        Fungal = null;
        isPickedUp = false;
    }

    public void PrepareThrow()
    {
        OnPrepareThrow?.Invoke();
    }

    public void PickUp(FungalController fungal)
    {
        Debug.Log("PickUp");
        HandlePickup(fungal);
        OnPickedUp?.Invoke();
    }

    public void HandlePickup(FungalController fungal)
    {
        Fungal = fungal;

        isPickedUp = true;

        audioSource.clip = audioClips.GetRandomItem();
        audioSource.pitch = audioPitch;
        audioSource.Play();

        Movement.SetSpeed(10);
        Movement.Follow(fungal.transform);
    }
}