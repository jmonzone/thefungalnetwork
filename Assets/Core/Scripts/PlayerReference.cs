using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class PlayerReference : ScriptableObject
{
    public Movement Movement { get; private set; }

    public event UnityAction OnPlayerUpdated;

    public void SetMovement(Movement movement)
    {
        Movement = movement;
        OnPlayerUpdated?.Invoke();
    }
}
