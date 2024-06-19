using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RandomMovement))]
public class FishController : MonoBehaviour
{
    [SerializeField] private FishData data;

    public FishData Data => data;

    public event UnityAction OnCaught;

    private enum FishState
    {
        IDLE,
        ATTRACTED
    }

    private FishState state;
    private MoveController movement;
    private RandomMovement randomMovement;

    private void Awake()
    {
        movement = GetComponent<MoveController>();
        randomMovement = GetComponent<RandomMovement>();
    }

    private void OnEnable()
    {
        transform.forward = Utility.RandomXZVector;
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

    private void Update()
    {
        switch (state)
        {
            case FishState.ATTRACTED:
                break;
        }
    }

    public void Initialize(FishData data, Collider bounds)
    {
        this.data = data;
        randomMovement.SetBounds(bounds);
    }

    public void Attract(Transform bob)
    {
        movement.SetTarget(bob.transform);
        SetState(FishState.ATTRACTED);
    }

    private void SetState(FishState state)
    {
        this.state = state;
        randomMovement.enabled = state == FishState.IDLE;
    }

}
