using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PassTheSpore : MonoBehaviour
{
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private PartyGuestSpawner guestManager;
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

    private List<UnitController> players = new List<UnitController>();

    private void Awake()
    {
        sporeMaterial = sporeOuterShell.material;
        Reset();

        exitButton.onClick.AddListener(EndGame);
    }

    public void StartGame()
    {
        virtualCamera.Priority = 11;
        sporeBall.gameObject.SetActive(true);

        players = unitManager.UnitControllers.Concat(guestManager.Guests).ToList();
        int count = players.Count;

        for (int i = 0; i < count; i++)
        {
            var ai = players[i].GetComponent<UnitAI>();

            // Evenly spaced angle around circle, but clockwise
            float angle = -(i / (float)count) * Mathf.PI * 2f;

            // Direction from center (clockwise order)
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
        var unitCount = players.Count;
        var currentUnitIndex = 0;
        sporeBall.position = players[currentUnitIndex % unitCount].transform.position + Vector3.up * 1f;

        while (true)
        {
            if (Input.GetMouseButtonUp(0))
            {
                currentUnitIndex++;

                Vector3 targetPos = players[currentUnitIndex % unitCount].transform.position + Vector3.up;
                yield return TossBall(sporeBall.position, targetPos, 0.5f);
            }
            yield return null;
        }
    }

    private IEnumerator TossBall(Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;

        // Optional: add an arc height for a nicer toss
        float arcHeight = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Lerp position
            Vector3 horizontal = Vector3.Lerp(start, end, t);

            // Add simple vertical arc (parabola)
            float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
            sporeBall.position = horizontal + Vector3.up * height;

            yield return null;
        }

        // Ensure final position is exact
        sporeBall.position = end;
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
        Reset();

        foreach (var unit in unitManager.UnitControllers)
        {
            var ai = unit.GetComponent<UnitAI>();
            ai.StartWander();
        }
    }

    private void Reset()
    {
        StopAllCoroutines();
        virtualCamera.Priority = 0;
        sporeBall.gameObject.SetActive(false);
    }
}
