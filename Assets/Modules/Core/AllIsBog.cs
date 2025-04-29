using System.Collections.Generic;
using UnityEngine;

public class AllIsBog : MonoBehaviour
{
    [SerializeField] private GameReference game;
    [SerializeField] private MoveCharacterJoystick joystick;
    [SerializeField] private List<Transform> powerUpAnchors;

    private void Awake()
    {
        game.Initialize(powerUpAnchors);

        foreach(var anchor in powerUpAnchors)
        {
            anchor.gameObject.SetActive(false);
        }
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
        initializeController.Initialize(game.ClientPlayer.Fungal.Fungal, game.StartGame);
    }
}