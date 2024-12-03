using UnityEngine;

public class PufferballTest : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PufferballPlayer player;

    private void Awake()
    {
        //inputManager.OnInteractionButtonClicked += () =>
        //{
        //    Debug.Log("Launching");
        //    player.LaunchBall();
        //};
    }

    private void Update()
    {
        inputManager.CanInteract(player.HasPufferball);
    }

}
