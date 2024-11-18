using System.Collections;
using UnityEngine;

public enum FishState
{
    IDLE,
    ATTRACTED,
    CAUGHT
}

[RequireComponent(typeof(MovementController))]
public class FishController : MonoBehaviour
{
    [SerializeField] private FishData data;

    public FishData Data => data;

    public FishState State { get; private set; }

    private MovementController movement;
    private float baseSpeed;
    private FishingBobController fishingBob;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
        baseSpeed = movement.Speed;
    }

    public void Initialize(FishData data, Collider bounds)
    {
        this.data = data;
        movement.SetBounds(bounds);
        movement.StartRandomMovement();
    }

    private void OnEnable()
    {
        if (movement.FaceForward) transform.forward = Utility.RandomXZVector;
        transform.localScale = Vector3.zero;


        IEnumerator LerpScale()
        {
            while (transform.localScale.x < 1)
            {
                var scale = transform.localScale.x + Time.deltaTime;
                transform.localScale = Vector3.one * scale;
                yield return null;
            }

            transform.localScale = Vector3.one;
        }

        StartCoroutine(LerpScale());
        SetState(FishState.IDLE);
    }

    private void SetState(FishState state)
    {
        State = state;

        switch (state)
        {
            case FishState.IDLE:
                movement.SetSpeed(baseSpeed);
                movement.StartRandomMovement();
                break;
            case FishState.ATTRACTED:
                movement.SetTarget(fishingBob.transform);
                break;
            case FishState.CAUGHT:
                movement.SetSpeed(fishingBob.ReelSpeed);
                break;
        }
    }

    private void Update()
    {
        switch (State)
        {
            case FishState.ATTRACTED:
                if (movement.IsAtDestination) SetState(FishState.CAUGHT);
                break;
        }
    }

    public void Attract(FishingBobController bob)
    {
        fishingBob = bob;
        SetState(FishState.ATTRACTED);
    }
}