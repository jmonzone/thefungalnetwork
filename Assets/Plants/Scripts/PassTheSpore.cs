using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassTheSpore : ActivityController
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

    private UnitController currentPlayer;
    public Vector3 AnchorPosition => gameCenter.transform.position + Vector3.up * 2.5f;

    private void Awake()
    {
        sporeMaterial = sporeOuterShell.material;
    }

    protected override IEnumerator OnActivityStart()
    {
        int currentUnitIndex = 0;
        var activePlayers = new List<UnitController>(Activity.Units);
        currentPlayer = GetNextActivePlayer(ref currentUnitIndex, ref activePlayers);

        sporeBall.position = currentPlayer.transform.position + Vector3.up;
        sporeBall.gameObject.SetActive(true);

        var t = 0f;
        while (true)
        {
            t += Time.deltaTime;
            sporeMaterial.SetColor("_Outer_Color", Color.Lerp(startColor, endColor, t / heatDuration));

            if (!isMidAir)
            {
                currentPlayer = GetNextActivePlayer(ref currentUnitIndex, ref activePlayers);
                Vector3 targetPos = currentPlayer.transform.position + Vector3.up;
                StartCoroutine(TossBall(sporeBall.position, targetPos, 0.5f));

                if (t >= heatDuration)
                {
                    activePlayers.Remove(currentPlayer);

                    if (activePlayers.Count <= 1)
                    {
                        yield return new WaitForSeconds(2f);
                        Activity.EndActivity();
                        yield break; // stop coroutine
                    }

                    // reset
                    t = 0f;
                    sporeMaterial.SetColor("_Outer_Color", startColor);
                }
            }

            yield return null;
        }
    }

    // Helper to get next player who isn't done
    private UnitController GetNextActivePlayer(ref int index, ref List<UnitController> players)
    {
        index = (index + 1) % players.Count;
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

        sporeBall.position = end;
        partyReference.IncrementScore(10, sporeBall.position);
        isMidAir = false;
    }

    protected override void OnActivityEnded()
    {
        base.OnActivityEnded();
        StopAllCoroutines();
        sporeBall.gameObject.SetActive(false);
    }

    
}
