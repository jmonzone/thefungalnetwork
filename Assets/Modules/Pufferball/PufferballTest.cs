using UnityEngine;

public class PufferballTest : MonoBehaviour
{
    [SerializeField] private MovementController movement;

    private void Start()
    {
        var direction = new Vector3(-1, 0, 1);
        movement.SetDirection(direction);
    }
}
