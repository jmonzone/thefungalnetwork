using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PassTheSpore : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private UnitManager unitManager;

    [Header("Gameplay")]
    [SerializeField] private Transform gameCenter;
    [SerializeField] private Transform sporeBall;
    [SerializeField] private Renderer sporeOuterShell;

    [Header("Settings")]
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private float heatDuration = 10f;

    private bool isMidAir = false;
    private Material sporeMaterial;

    private class PassTheSporePlayer
    {
        public FungalController Fungal;
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
    }

    public void StartGame()
    {
        partyReference.PauseParty();

        //virtualCamera.Priority = 11;
        sporeBall.gameObject.SetActive(true);

        foreach(var unit in unitManager.AllUnits)
        {
            players.Add(new PassTheSporePlayer
            {
                Fungal = unit as FungalController,
                isDone = false,
            });
        };

        int count = players.Count;

        for (int i = 0; i < count; i++)
        {
            var fungalController = players[i].Fungal.GetComponent<FungalController>();

            // Evenly spaced angle around circle, but clockwise
            float angle = -(i / (float)count) * Mathf.PI * 2f;

            // Direction from center (clockwise order)
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            // Position offset outward from center
            Vector3 destination = gameCenter.position + direction * 1f;

            fungalController.SetDestination(destination);
            fungalController.SetLookPosition(gameCenter.transform.position);
        }

        //cameraPanController.CenterTargetInView(gameCenter.position);
        //navigation.Navigate(passTheSporeView);

        StartCoroutine(GameInput());
        StartCoroutine(GameUpdate());

        OnGameStart?.Invoke();
    }

    private IEnumerator GameInput()
    {
        Debug.Log("Waiting");
        yield return new WaitUntil(() => players.All(player => player.Fungal.IsAtDestination));
        Debug.Log("Waiting complete");

        int currentUnitIndex = 0;

        // Find first active player
        currentPlayer = GetNextActivePlayer(ref currentUnitIndex);
        sporeBall.position = currentPlayer.Fungal.transform.position + Vector3.up;

        while (true)
        {
            yield return new WaitForSeconds(1f);

            // Move to next active player
            currentPlayer = GetNextActivePlayer(ref currentUnitIndex);

            Vector3 targetPos = currentPlayer.Fungal.transform.position + Vector3.up;
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
        partyReference.IncrementScore(10);
        isMidAir = false;
    }
    private IEnumerator GameUpdate()
    {
        float elapsed = 0f;

        // Wait until all players are at destination OR timeout reached
        yield return new WaitUntil(() =>
        {
            elapsed += Time.deltaTime;
            return players.All(player => player.Fungal.IsAtDestination) || elapsed >= 2f;
        });

        sporeMaterial.SetColor("_Outer_Color", startColor);

        var t = 0f;
        while (true)
        {
            sporeMaterial.SetColor("_Outer_Color", Color.Lerp(startColor, endColor, t / heatDuration));
            t += Time.deltaTime;

            if (!isMidAir && t >= heatDuration)
            {
                // trigger your code when fully heated
                currentPlayer.Fungal.TriggerDeath();
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

        Reset();

        foreach (var player in players)
        {
            var fungalController = player.Fungal.GetComponent<FungalController>();
            fungalController.SetDefaultBehaviour();
            fungalController.TriggerRespawn();
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
