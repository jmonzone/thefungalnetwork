using UnityEngine;

// handles the behaviour of the fishing rod bob
public class FishingBobController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float gravity = 0.1f;
    [SerializeField] private float reelSpeed = 2f;

    public Rigidbody Rigidbody => rigidbody;
    public Vector3 StartPosition { get; private set; }

    private FishingRodState state;

    public bool IsReeledIn => Vector3.Dot(Rigidbody.velocity, StartPosition - transform.position) <= 0;

    private void Awake()
    {
        StartPosition = transform.position;
    }

    private void OnEnable()
    {
        SetState(FishingRodState.IDLE);
    }

    public void SetState(FishingRodState state)
    {
        this.state = state;
        Rigidbody.useGravity = state == FishingRodState.IN_AIR;

        switch (state)
        {
            // disable movement and place bob at start
            case FishingRodState.IDLE:
                Rigidbody.velocity = Vector3.zero;
                Rigidbody.MovePosition(StartPosition);
                break;

            // disable movement when bob lands
            case FishingRodState.IN_WATER:
                Rigidbody.velocity = Vector3.zero;

                var position = transform.position;
                position.y = 0;
                Rigidbody.MovePosition(position);
                break;

            // move bob back to center
            case FishingRodState.REELING:
                var direction = StartPosition - transform.position;
                Rigidbody.velocity = direction.normalized * reelSpeed;
                break;
        }
    }

    private void Update()
    {
        switch (state)
        {
            // add additional gravity force
            case FishingRodState.IN_AIR:
                if (transform.position.y > 0)
                {
                    Rigidbody.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
                }
                break;
        }
    }
}
