using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassTheSpore : ActivityController
{
    [Header("References")]
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private Skill sporeSkill;
    [SerializeField] private Transform sporeBall;

    protected override IEnumerator OnActivityStart()
    {
        int currentUnitIndex = 0;
        var activePlayers = new List<UnitController>(Activity.Units);
        var currentPlayer = GetNextActivePlayer(ref currentUnitIndex, ref activePlayers);

        sporeBall.position = currentPlayer.transform.position + Vector3.up;
        sporeBall.gameObject.SetActive(true);

        while (true)
        {
            currentPlayer = GetNextActivePlayer(ref currentUnitIndex, ref activePlayers);

            IncreaseXP(currentPlayer, 3);
            Vector3 targetPos = currentPlayer.transform.position + Vector3.up;
            yield return TossBall(sporeBall.position, targetPos, djReference.BeatDuration);
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
    }

    protected override void OnActivityEnded()
    {
        base.OnActivityEnded();
        StopAllCoroutines();
        sporeBall.gameObject.SetActive(false);
    }
}
