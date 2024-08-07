using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerTest : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private GameObject connectUI;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button connectButton;

    private Transform player;

    private void Start()
    {
        connectButton.onClick.AddListener(() =>
        {
            multiplayerManager.AutoConnect(nameInputField.text);
            connectUI.SetActive(false);
            virtualJoystick.gameObject.SetActive(true);
        });

        virtualJoystick.gameObject.SetActive(false);
        virtualJoystick.OnJoystickUpdate += direction =>
        {
            if (!player) return;
            var mappedDirection = new Vector3(direction.x, 0, direction.y);
            player.transform.position += mappedDirection;
        };

        NetworkPlayer.OnLocalPlayerSpawned += player =>
        {
            this.player = player;
            cameraController.Target = player;
        };
    }
}
