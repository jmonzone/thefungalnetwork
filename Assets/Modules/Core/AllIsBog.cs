using UnityEngine;

public class AllIsBog : MonoBehaviour
{
    [SerializeField] private GameReference game;
    [SerializeField] private MoveCharacterJoystick joystick;

    private void Awake()
    {
        game.Initialize();
    }

    private void OnEnable()
    {
        game.OnAllPlayersAdded += OnAllPlayersAdded;
    }

    private void OnDisable()
    {
        game.OnAllPlayersAdded -= OnAllPlayersAdded;
    }

    private void OnAllPlayersAdded()
    {
        joystick.player = game.ClientPlayer.Fungal.Movement;

        var initializeController = GetComponent<InitializeController>();
        initializeController.Initialize(game.ClientPlayer.Fungal.Fungal);
    }
}