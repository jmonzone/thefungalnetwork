using UnityEngine;
using UnityEngine.UI;

public class DancefloorIntroUI : MonoBehaviour
{
    [SerializeField] private DancefloorReference dancefloorReference;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        readyButton.onClick.AddListener(dancefloorReference.StartDancefloor);
        exitButton.onClick.AddListener(dancefloorReference.ExitDancefloor);
    }
}
