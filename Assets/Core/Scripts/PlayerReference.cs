using System;
using UnityEngine;
using UnityEngine.Events;

[Obsolete]
[CreateAssetMenu]
public class PlayerReference : ScriptableObject
{
    public NetworkFungal Fungal { get; private set; }
    public Movement Movement => Fungal.Movement;

    public event UnityAction OnPlayerUpdated;

    public void SetMovement(NetworkFungal fungal)
    {
        Fungal = fungal;
        OnPlayerUpdated?.Invoke();
    }
}
