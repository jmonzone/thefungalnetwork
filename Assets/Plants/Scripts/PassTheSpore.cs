using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PassTheSpore : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private PartyGuestSpawner guestManager;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference passTheSporeView;
    [SerializeField] private Transform gameCenter;
    [SerializeField] private Button exitButton;
    [SerializeField] private Transform sporeBall;
    [SerializeField] private Renderer sporeOuterShell;
    [SerializeField] private CameraPanController cameraPanController;

    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;

    [SerializeField] private float heatDuration = 10f;
    private bool isMidAir = false;

    private Material sporeMaterial;

    private class PassTheSporePlayer
    {
        public UnitController unit;
        public bool isDone;
    }

    private PassTheSporePlayer currentPlayer;
    private List<PassTheSporePlayer> players = new List<PassTheSporePlayer>();
    public Vector3 AnchorPosition => gameCenter.transform.position + Vector3.up * 2.5f;

    public event UnityAction OnGameStart;
    public event UnityAction OnGameComplete;

    private void Awake()
    {
        sporeMaterial = sporeOuterShell.material;
        Reset();

        exitButton.onClick.AddListener(EndGame);
    }

    public void StartGame()
    {
        partyReference.PauseParty();

        //virtualCamera.Priority = 11;
        sporeBall.gameObject.SetActive(true);

        foreach(var unit in unitManager.UnitControllers.Concat(partyReference.Guests))
        {
            players.Add(new PassTheSporePlayer
            {
                unit = unit,
                isDone = false,
            });
        };

        int count = players.Count;

        for (int i = 0; i < count; i++)
        {
            var ai = players[i].unit.GetComponent<FungalController>();

            // Evenly spaced angle around circle, but clockwise
            float angle = -(i / (float)count) * Mathf.PI * 2f;

            // Direction from center (clockwise order)
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            // Position offset outward from center
            Vector3 destination = gameCenter.position + direction * 1f;

            // Assign destination + facing direction
            ai.SetDestination(destination, -direction); // face toward center
        }

        cameraPanController.CenterTargetInView(gameCenter.position);
        //navigation.Navigate(passTheSporeView);

        StartCoroutine(GameInput());
        StartCoroutine(GameUpdate());

        OnGameStart?.Invoke();
    }

    private IEnumerator GameInput()
    {
        int currentUnitIndex = 0;

        // Find first active player
        currentPlayer = GetNextActivePlayer(ref currentUnitIndex);
        sporeBall.position = currentPlayer.unit.transform.position + Vector3.up;

        while (true)
        {
            yield return new WaitForSeconds(1f);

            //if (Input.GetMouseButtonDown(0) || currentPlayer.isDone)
            //{
               
            //}

            // Move to next active player
            currentPlayer = GetNextActivePlayer(ref currentUnitIndex);

            Vector3 targetPos = currentPlayer.unit.transform.position + Vector3.up;
            yield return TossBall(sporeBall.position, targetPos, 0.5f);
        }
    }

    // Helper to get next player who isn't done
    private PassTheSporePlayer GetNextActivePlayer(ref int index)
    {
        int startIndex = index;

        do
        {
            index = (index + 1) % players.Count;
            if (!players[index].isDone) return players[index];

        } while (index != startIndex); // looped all players

        // Fallback if everyone is done
        return players[index];
    }


    private IEnumerator TossBall(Vector3 start, Vector3 end, float duration)
    {
        isMidAir = true;
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

        isMidAir = false;
    }
    private IEnumerator GameUpdate()
    {
        sporeMaterial.SetColor("_Outer_Color", startColor);

        var t = 0f;
        while (true)
        {
            sporeMaterial.SetColor("_Outer_Color", Color.Lerp(startColor, endColor, t / heatDuration));
            t += Time.deltaTime;

            if (!isMidAir && t >= heatDuration)
            {
                // trigger your code when fully heated
                //currentPlayer.unit.GetComponent<UnitAnimation>().PlayAnimation("Death");
                currentPlayer.isDone = true;

                // Check if only one player is left
                int activePlayers = players.Count(p => !p.isDone);
                if (activePlayers <= 1)
                {
                    yield return new WaitForSeconds(2f);
                    EndGame();
                    yield break; // stop coroutine
                }

                // reset
                t = 0f;
                sporeMaterial.SetColor("_Outer_Color", startColor);
            }

            yield return null;
        }
    }

    private void EndGame()
    {
        partyReference.ResumeParty();

        //navigation.GoBack();

        Reset();

        foreach (var player in players)
        {
            //var animation = player.unit.GetComponent<UnitAnimation>();
            //animation.TriggerRespawn();

            //var ai = player.unit.GetComponent<FungalController>();
            //ai.StopActivity();
        }

        players = new List<PassTheSporePlayer>();

        OnGameComplete?.Invoke();
    }

    private void Reset()
    {
        StopAllCoroutines();
        //virtualCamera.Priority = 0;
        sporeBall.gameObject.SetActive(false);
    }
}
