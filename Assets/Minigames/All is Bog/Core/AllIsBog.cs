using Unity.Netcode;
using UnityEngine;

public class AllIsBog : MonoBehaviour
{
    [SerializeField] private PufferballReference pufferballReference;

    private void Awake()
    {
        pufferballReference.Initialize();
    }
}
