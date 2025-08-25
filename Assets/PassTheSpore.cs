using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PassTheSpore : MonoBehaviour
{
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference passTheSporeView;
    [SerializeField] private Transform gameCenter;
    [SerializeField] private Button exitButton;
    [SerializeField] private Transform sporeBall;
    [SerializeField] private Renderer sporeOuterShell;


    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;

    private Material sporeMaterial;

    private void Awake()
    {
        sporeMaterial = sporeOuterShell.material;
        EndGame();

        exitButton.onClick.AddListener(EndGame);
    }

    public void StartGame()
    {
        virtualCamera.Priority = 11;

        var units = unitManager.UnitControllers;
        int count = units.Count;

        for (int i = 0; i < count; i++)
        {
            var ai = units[i].GetComponent<UnitAI>();

            // Evenly spaced angle around a circle
            float angle = (i / (float)count) * Mathf.PI * 2f;

            // Direction from center
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            // Position offset outward from center
            Vector3 destination = gameCenter.transform.position + direction * 1f;

            // Assign destination + facing direction
            ai.SetDestination(destination, -direction); // face toward center
        }

        navigation.Navigate(passTheSporeView);

        StartCoroutine(GameInput());
        StartCoroutine(GameUpdate());
    }

    private IEnumerator GameInput()
    {
        var unitCount = unitManager.UnitControllers.Count;
        var currentUnitIndex = 0;

        while (true)
        {
            sporeBall.position = unitManager.UnitControllers[currentUnitIndex % unitCount].transform.position + Vector3.up * 1f;
            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
            yield return new WaitForEndOfFrame();
            currentUnitIndex++;
        }
    }

    private IEnumerator GameUpdate()
    {
        sporeMaterial.SetColor("_Outer_Color", startColor);

        var i = 0f;
        while (true)
        {
            sporeMaterial.SetColor("_Outer_Color", Color.Lerp(startColor, endColor, i / 10f));
            i += Time.deltaTime;

            if (i > 10f) i = 0;
            yield return null;
        }
    }
    private void EndGame()
    {
        StopAllCoroutines();
        virtualCamera.Priority = 0;

        foreach(var unit in unitManager.UnitControllers)
        {
            var ai = unit.GetComponent<UnitAI>();
            ai.StartWander();
        }
    }
}
