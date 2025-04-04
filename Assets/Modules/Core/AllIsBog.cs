using Unity.Netcode;
using UnityEngine;

public class AllIsBog : MonoBehaviour
{
    [SerializeField] private GameReference pufferballReference;

    private void Awake()
    {
        pufferballReference.Initialize();
    }
}
