using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private GameReference pufferball;
    [SerializeField] private List<Color> colors;

    [SerializeField] private RectTransform barContainer;
    [SerializeField] private ScoreSegment segmentPrefab;

    private List<ScoreSegment> segmentControllers = new List<ScoreSegment>();

    private void Awake()
    {
        GetComponentsInChildren(includeInactive: true, segmentControllers);
    }

    private void Start()
    {
        PufferballMinigame_OnScoreUpdated();
    }

    private void OnEnable()
    {
        pufferball.OnScoreUpdated += PufferballMinigame_OnScoreUpdated;
    }

    private void OnDisable()
    {
        pufferball.OnScoreUpdated -= PufferballMinigame_OnScoreUpdated;
    }


    public void PufferballMinigame_OnScoreUpdated()
    {
        Debug.Log("PufferballMinigame_OnScoreUpdated");

        var clientPlayer = pufferball.ClientPlayer; // Reference your client player

        // Move client player to the front if necessary
        int clientIndex = clientPlayer.Index;


        var players = pufferball.Players;

        var orderedPlayers = new List<Player>(players);

        if (clientIndex >= orderedPlayers.Count) return;


        if (clientIndex > 0)
        {
            orderedPlayers[clientIndex] = orderedPlayers[0];
            orderedPlayers[0] = clientPlayer;
        }

        float barWidth = barContainer.rect.width;
        const float totalScore = 100f; // Assuming total score is fixed (or adjust dynamically if needed)

        for (int i = 0; i < orderedPlayers.Count; i++)
        {
            var player = orderedPlayers[i];
            float normalizedScore = player.Score / totalScore;

            // Instantiate segment if it doesn't exist yet
            if (i >= segmentControllers.Count)
            {
                var segmentController = Instantiate(segmentPrefab, barContainer);
                segmentControllers.Add(segmentController);
            }

            var segment = segmentControllers[i];
            segment.gameObject.SetActive(true);

            // Assign fungal and color once
            segment.SetFungal(player.Fungal);

            // Set segment width
            segment.SetTargetWidth(barWidth * normalizedScore);
        }

        // Deactivate any unused segments
        for (int i = orderedPlayers.Count; i < segmentControllers.Count; i++)
        {
            segmentControllers[i].gameObject.SetActive(false);
        }
    }

}
